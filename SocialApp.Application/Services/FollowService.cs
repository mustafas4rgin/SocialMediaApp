using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using SocialApp.Application.Interfaces;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;
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

            var alreadyExists = await _followRepository
                .GetAllActive()
                .AsNoTracking()
                .AnyAsync(f => f.FollowerId == follow.FollowerId && f.FollowingId == follow.FollowingId, ct);

            if (alreadyExists)
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