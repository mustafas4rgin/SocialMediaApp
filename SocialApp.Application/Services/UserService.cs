using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using SocialApp.Application.Helpers;
using SocialApp.Application.Interfaces;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.DTOs;
using SocialApp.Domain.Entities;
using SocialApp.Domain.Parameters;
using SocialApp.Domain.Results.Error;
using SocialApp.Domain.Results.Success;

namespace SocialApp.Application.Services;

public class UserService : GenericService<User>, IUserService
{
    private readonly IDistributedCache _cache;
    private readonly IUserRepository _userRepository;
    private readonly ILogger _logger;
    public UserService(
    IDistributedCache cache,
    IValidator<User> validator,
    IUserRepository userRepository,
    ILogger<UserService> logger
    ) : base(validator, userRepository, logger)
    {
        _cache = cache;
        _userRepository = userRepository;
        _logger = logger;
    }
    public async Task<IServiceResultWithData<List<UserRecommendationDto>>> GetRecommendedUsersAsync(int userId, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        try
        {
            var recommendedUsers = await _userRepository.GetRecommendedUsersAsync(userId, pageNumber, pageSize, ct);

            if (!recommendedUsers.Any())
                return new ErrorResultWithData<List<UserRecommendationDto>>("There is no recommendation.", 404);
            
            return new SuccessResultWithData<List<UserRecommendationDto>>("Recommendations: ", recommendedUsers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occured while getting recommended users.");
            return new ErrorResultWithData<List<UserRecommendationDto>>("An error occured while getting recommended users.");
        }
    }
}
