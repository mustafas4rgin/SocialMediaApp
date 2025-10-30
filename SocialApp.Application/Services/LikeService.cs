using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using SocialApp.Application.Helpers;
using SocialApp.Application.Registrations;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;
using SocialApp.Domain.Parameters;
using SocialApp.Domain.Results.Error;
using SocialApp.Domain.Results.Success;

namespace SocialApp.Application.Services;

public class LikeService : GenericService<Like>, ILikeService
{
    private readonly IDistributedCache _cache;
    private readonly IValidator<Like> _validator;
    private readonly ILikeRepository _likeRepository;
    private readonly ILogger<LikeService> _logger;
    private static readonly SemaphoreSlim _sfLock = new(1, 1);
    public LikeService(
    IValidator<Like> validator,
    IDistributedCache cache,
    ILikeRepository likeRepository,
    ILogger<LikeService> logger
    ) : base(validator, likeRepository, logger)
    {
        _cache = cache;
        _validator = validator;
        _likeRepository = likeRepository;
        _logger = logger;
    }
    public async Task<IServiceResultWithData<IEnumerable<Like>>> GetAllLikesWithIncludesAsync(QueryParameters param, CancellationToken ct = default)
    {
        try
        {
            var cacheKey = ListHelper.LikesListKey(param);

            var cached = await CacheHelper.GetTypedAsync<IEnumerable<Like>>(_cache, cacheKey, ct);
            if (cached is not null && cached.Any())
                return new SuccessResultWithData<IEnumerable<Like>>("Roles (cache)", cached);

            //anti-stampede
            await _sfLock.WaitAsync(ct);
            try
            {
                //double check
                cached = await CacheHelper.GetTypedAsync<IEnumerable<Like>>(_cache, cacheKey, ct);
                if (cached is not null && cached.Any())
                    return new SuccessResultWithData<IEnumerable<Like>>("Likes (cache)", cached);

                var query = _likeRepository.GetAllActive(ct);

                if (!string.IsNullOrEmpty(param.Include))
                    query = QueryHelper.ApplyIncludesForLike(query, param.Include);

                var likes = await query.ToListAsync(ct);

                if (!likes.Any())
                    return new ErrorResultWithData<IEnumerable<Like>>("There is no like.");

                //caching
                await CacheHelper.SetTypedAsync(_cache, cacheKey, likes, ct);

                return new SuccessResultWithData<IEnumerable<Like>>("Likes found.", likes);
            }
            finally
            {
                _sfLock.Release();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occured while getting likes.");
            return new ErrorResultWithData<IEnumerable<Like>>(ex.Message);
        }
    }
    public async Task<IServiceResultWithData<Like>> GetLikeByIdWithIncludesAsync(int id, QueryParameters param, CancellationToken ct = default)
    {
        try
        {
            var query = _likeRepository.GetAllActive(ct);

            if (!string.IsNullOrEmpty(param.Include))
                query = QueryHelper.ApplyIncludesForLike(query, param.Include);

            var like = await query.FirstOrDefaultAsync(l => l.Id == id, ct);

            if (like is null)
                return new ErrorResultWithData<Like>($"There is no like with ID : {id}");

            return new SuccessResultWithData<Like>("Like found.", like);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occured while getting like with ID : {id}");
            return new ErrorResultWithData<Like>(ex.Message);
        }
    }
    public override async Task<IServiceResult> AddAsync(Like like, CancellationToken ct = default)
    {
        if (like is null)
            return new ErrorResult("Bad request.");

        var validationResult = await _validator.ValidateAsync(like, ct);

        if (!validationResult.IsValid)
            return new ErrorResult(string.Join(" | ",
                validationResult.Errors.Select(e => e.ErrorMessage)));

        try
        {
            var existingLike = await _likeRepository.GetExistLikeAsync(like.PostId, like.UserId, ct);

            if (existingLike is not null)
            {
                _likeRepository.Delete(existingLike, ct);
                await _likeRepository.SaveChangesAsync(ct);
                return new SuccessResult("Unliked successfully.");
            }

            await _likeRepository.AddAsync(like, ct);
            await _likeRepository.SaveChangesAsync(ct);

            return new SuccessResult("Liked successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while liking post {PostId} by user {UserId}", like.PostId, like.UserId);
            return new ErrorResult(ex.Message);
        }
    }
}