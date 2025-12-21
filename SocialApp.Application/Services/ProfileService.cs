using Microsoft.Extensions.Logging;
using SocialApp.Application.Interfaces;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.DTOs;
using SocialApp.Domain.Entities;
using SocialApp.Domain.Parameters;
using SocialApp.Domain.Results.Error;
using SocialApp.Domain.Results.Success;
using StackExchange.Redis;

namespace SocialApp.Application.Services;

public class ProfileService : IProfileService
{
    private readonly IUserRepository _userRepository;
    private readonly IPostRepository _postRepository;
    private readonly ILogger<ProfileService> _logger;
    public ProfileService
    (
        IPostRepository postRepository,
        IUserRepository userRepository,
        ILogger<ProfileService> logger
    )
    {
        _postRepository = postRepository;
        _logger = logger;
        _userRepository = userRepository;
    }
    public async Task<IServiceResultWithData<ProfileDTO>> GetProfileAsync(int userId, QueryParameters param, CancellationToken ct = default)
    {
        var profileHeader = await _userRepository.GetProfileHeaderAsync(userId, ct);

        if (profileHeader is null)
            return new ErrorResultWithData<ProfileDTO>("User not found.", 404);
        
        var usersPosts = await _postRepository.GetUserPostsPagedAsync(userId, param.PageNumber, param.PageSize, ct);
        var usersPostsCount = await _postRepository.CountUsersPostsAsync(userId, ct);
        
        try
        {
            return new SuccessResultWithData<ProfileDTO>("Profile fetched successfully.",
            new ProfileDTO
            {
                HeaderDTO = profileHeader,
                Posts = usersPosts,
                PostsCount = usersPosts.Count()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occured while getting user.");
            return new ErrorResultWithData<ProfileDTO>("An error occured while getting user.");
        }
    }
}