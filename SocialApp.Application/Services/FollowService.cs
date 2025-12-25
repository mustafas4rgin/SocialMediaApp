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
    public async Task<IServiceResultWithData<IEnumerable<Follow>>> GetFollowsByFollowingId(int followingId, QueryParameters param, CancellationToken ct = default)
    {
        try
        {
            var follows = await _followRepository.GetFollowsByFollowingIdAsync(followingId, ct);

            if (!follows.Any())
                return new ErrorResultWithData<IEnumerable<Follow>>("There is no follow.", 404);

            return new SuccessResultWithData<IEnumerable<Follow>>("Followers: ", follows);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while getting follows.");
            return new ErrorResultWithData<IEnumerable<Follow>>("An unexpected error occured.");
        }
    }
    public override async Task<IServiceResultWithData<Follow>> AddAsync(Follow follow, CancellationToken ct)
    {
        if (follow is null)
            return new ErrorResultWithData<Follow>("Bad request.");

        if (follow.FollowerId == follow.FollowingId)
            return new ErrorResultWithData<Follow>("You can't follow yourself.", 409);

        var validationResult = await _validator.ValidateAsync(follow, ct);

        if (!validationResult.IsValid)
            return new ErrorResultWithData<Follow>(string.Join(" | ",
                validationResult.Errors.Select(e => e.ErrorMessage)));

        try
        {

            var existFollow = await _followRepository.GetExistFollowAsync(follow.FollowerId, follow.FollowingId, ct);

            if (existFollow is not null)
                return new ErrorResultWithData<Follow>("Already follows.", 409);

            await _followRepository.AddAsync(follow, ct);
            await _followRepository.SaveChangesAsync();

            return new SuccessResultWithData<Follow>("Followed successfully.", follow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new ErrorResultWithData<Follow>("An unexpected error occured.");
        }
    }
}
