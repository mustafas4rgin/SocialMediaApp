using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using SocialApp.Application.Helpers;
using SocialApp.Application.Interfaces;
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
    private const string LikesCachePrefix = "likes:";

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

    public async Task<IServiceResultWithData<IEnumerable<Like>>> GetLikesByUserIdAsync(
        int userId,
        QueryParameters param,
        CancellationToken ct = default)
    {
        try
        {
            param ??= new QueryParameters();

            var query = _likeRepository.GetAllByUserId(userId, ct);

            if (!string.IsNullOrEmpty(param.Include))
                query = QueryHelper.ApplyIncludesForLike(query, param.Include);

            query = query.AsNoTracking();

            var likes = await query.ToListAsync(ct);

            if (!likes.Any())
                return new ErrorResultWithData<IEnumerable<Like>>("There is no like.");

            return new SuccessResultWithData<IEnumerable<Like>>("Likes found.", likes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "An error occured while getting likes for user {UserId}.",
                userId);

            return new ErrorResultWithData<IEnumerable<Like>>("An unexpected error occured.");
        }
    }

    public async Task<IServiceResultWithData<IEnumerable<Like>>> GetAllLikesWithIncludesAsync(
        QueryParameters param,
        CancellationToken ct = default)
    {
        try
        {
            param ??= new QueryParameters();

            var cacheKey = ListHelper.LikesListKey(param);

            var cached = await CacheHelper.GetTypedAsync<IEnumerable<Like>>(_cache, cacheKey, ct);
            if (cached is not null && cached.Any())
                return new SuccessResultWithData<IEnumerable<Like>>("Likes (cache)", cached);

            // anti-stampede
            await _sfLock.WaitAsync(ct);

            try
            {
                // double-check cache
                cached = await CacheHelper.GetTypedAsync<IEnumerable<Like>>(_cache, cacheKey, ct);
                if (cached is not null && cached.Any())
                    return new SuccessResultWithData<IEnumerable<Like>>("Likes (cache)", cached);

                var query = _likeRepository.GetAllActive(ct);

                if (!string.IsNullOrEmpty(param.Include))
                    query = QueryHelper.ApplyIncludesForLike(query, param.Include);

                query = query.AsNoTracking();

                var likes = await query.ToListAsync(ct);

                if (!likes.Any())
                    return new ErrorResultWithData<IEnumerable<Like>>("There is no like.");

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
            return new ErrorResultWithData<IEnumerable<Like>>("An unexpected error occured.");
        }
    }

    public async Task<IServiceResultWithData<Like>> GetLikeByIdWithIncludesAsync(
        int id,
        QueryParameters param,
        CancellationToken ct = default)
    {
        try
        {
            param ??= new QueryParameters();

            var cacheKey = GetById.Like(id, param.Include); 

            var cached = await CacheHelper.GetTypedAsync<Like>(_cache, cacheKey, ct);
            if (cached is not null)
                return new SuccessResultWithData<Like>("Like (cache)", cached);

            var query = _likeRepository.GetAllActive(ct);

            if (!string.IsNullOrEmpty(param.Include))
                query = QueryHelper.ApplyIncludesForLike(query, param.Include);

            query = query.AsNoTracking();

            var like = await query.FirstOrDefaultAsync(l => l.Id == id, ct);

            if (like is null)
                return new ErrorResultWithData<Like>($"There is no like with ID: {id}");

            await CacheHelper.SetTypedAsync(_cache, cacheKey, like, ct);

            return new SuccessResultWithData<Like>("Like found.", like);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "An error occured while getting like with ID: {Id}",
                id);

            return new ErrorResultWithData<Like>("An unexpected error occured.");
        }
    }

    public override async Task<IServiceResult> AddAsync(
        Like like,
        CancellationToken ct = default)
    {
        if (like is null)
            return new ErrorResult("Bad request.");

        var validationResult = await _validator.ValidateAsync(like, ct);

        if (!validationResult.IsValid)
        {
            var errors = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
            return new ErrorResult(errors);
        }

        try
        {
            var existingLike = await _likeRepository.GetExistLikeAsync(
                like.PostId,
                like.UserId,
                ct);

            if (existingLike is not null)
            {
                _likeRepository.Delete(existingLike, ct);
                await _likeRepository.SaveChangesAsync(ct);
                await CacheHelper.RemoveByPatternAsync(_cache, LikesCachePrefix, ct);

                _logger.LogInformation(
                    "User {UserId} unliked post {PostId}.",
                    like.UserId,
                    like.PostId);

                return new SuccessResult("Unliked successfully.");
            }

            await _likeRepository.AddAsync(like, ct);
            await _likeRepository.SaveChangesAsync(ct);

            await CacheHelper.RemoveByPatternAsync(_cache, LikesCachePrefix, ct);

            _logger.LogInformation(
                "User {UserId} liked post {PostId}.",
                like.UserId,
                like.PostId);

            return new SuccessResult("Liked successfully.");
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx,
                "DbUpdateException occurred while liking/unliking post {PostId} by user {UserId}.",
                like.PostId,
                like.UserId);

            return new ErrorResult("An error occurred while updating like state.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error occurred while liking/unliking post {PostId} by user {UserId}.",
                like.PostId,
                like.UserId);

            return new ErrorResult("An unexpected error occured.");
        }
    }
}
