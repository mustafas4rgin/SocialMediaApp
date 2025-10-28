using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using SocialApp.Application.Helpers;
using SocialApp.Application.Interfaces;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;
using SocialApp.Domain.Parameters;
using SocialApp.Domain.Results.Error;
using SocialApp.Domain.Results.Success;

namespace SocialApp.Application.Services;

public class FollowService : GenericService<Follow>, IFollowService
{
    private readonly IValidator<Follow> _validator;
    private readonly IFollowRepository _followRepository;
    private readonly ILogger<FollowService> _logger;
    public FollowService(
    IValidator<Follow> validator,
    IFollowRepository repository,
    ILogger<FollowService> logger
    ) : base(validator, repository, logger)
    {
        _logger = logger;
        _validator = validator;
        _followRepository = repository;
    }
    public async Task<IServiceResultWithData<IEnumerable<Follow>>> GetAllFollowsWithIncludesAsync(QueryParameters param, CancellationToken ct = default)
    {
        try
        {
            var query = _followRepository.GetAllActive(ct);

            if (!string.IsNullOrEmpty(param.Include))
                query = QueryHelper.ApplyIncludesForFollow(query, param.Include);

            var follows = await query.ToListAsync(ct);

            if (!follows.Any())
                return new ErrorResultWithData<IEnumerable<Follow>>("There is no follow.");

            return new SuccessResultWithData<IEnumerable<Follow>>("Follows found.", follows);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while getting follows.");
            return new ErrorResultWithData<IEnumerable<Follow>>("An error occured.");
        }
    }
    public async Task<IServiceResultWithData<Follow>> GetFollowByIdWithIncludesAsync(int id, QueryParameters param, CancellationToken ct = default)
    {
        try
        {
            var query = _followRepository.GetAllActive(ct);

            if (!string.IsNullOrEmpty(param.Include))
                query = QueryHelper.ApplyIncludesForFollow(query, param.Include);

            var follow = await query.FirstOrDefaultAsync(f => f.Id == id, ct);

            if (follow is null)
                return new ErrorResultWithData<Follow>($"There is no follow with ID : {id}");

            return new SuccessResultWithData<Follow>("Follow found.", follow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occured while getting follow with ID : {id}");
            return new ErrorResultWithData<Follow>(ex.Message);
        }
    }
    public override async Task<IServiceResult> AddAsync(Follow follow, CancellationToken ct)
    {
        if (follow is null)
            return new ErrorResult("Bad request.");

        if (follow.FollowerId == follow.FollowingId)
            return new ErrorResult("You can't follow yourself.");

        var validationResult = await _validator.ValidateAsync(follow, ct);

        if (!validationResult.IsValid)
            return new ErrorResult(string.Join(" | ",
                validationResult.Errors.Select(e => e.ErrorMessage)));

        try
        {

            var existFollow = await _followRepository.GetExistFollowAsync(follow.FollowerId, follow.FollowingId, ct);

            if (existFollow is not null)
                return new ErrorResult("Already follows.");

            await _followRepository.AddAsync(follow, ct);
            await _followRepository.SaveChangesAsync();

            return new SuccessResult("Followed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new ErrorResult(ex.Message);
        }
    }
}