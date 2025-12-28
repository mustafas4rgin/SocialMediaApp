using System.Formats.Asn1;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialApp.Application.Interfaces;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.DTOs;
using SocialApp.Domain.Entities;
using SocialApp.Domain.Events;
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
    private readonly IMediator _mediator;
    public ProfileService
    (
        IPostRepository postRepository,
        IUserRepository userRepository,
        ILogger<ProfileService> logger,
        IMediator mediator
    )
    {
        _mediator = mediator;
        _postRepository = postRepository;
        _logger = logger;
        _userRepository = userRepository;
    }
    public async Task<IServiceResult> UpdateProfileAsync(int userId, UpdateProfileDTO dTO, CancellationToken ct = default)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(
                id: userId,
                includeDeleted: false,
                asNoTracking: false,
                ct: ct
            );

            if (user is null) return new ErrorResult("User not found.");

            if (dTO.UserName != null && dTO.UserName != user.UserName)
            {
                if (await _userRepository.UserExistsByUsernameAsync(dTO.UserName, ct))
                    return new ErrorResult("Username already taken.");

                user.UserName = dTO.UserName;
            }

            if (dTO.FirstName != null) user.FirstName = dTO.FirstName;

            if (dTO.LastName != null) user.LastName = dTO.LastName;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync(ct);

            try
            {
                await _mediator.Publish(new UpdateProfileEvent(userId: userId), ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred.");
                return new ErrorResult("Unexpected error occured while sending notification.");
            }

            return new SuccessResult("Profile updated successfully.");

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred.");
            return new ErrorResult("An unexpected error occurred while updating profile.");
        }
    }
    public async Task<IServiceResultWithData<ProfileDTO>> GetProfileWithUsernameAsync(string userName, QueryParameters param, CancellationToken ct = default)
    {
        var profileHeader = await _userRepository.GetProfileByUsernameAsync(userName, ct);

        if (profileHeader is null)
            return new ErrorResultWithData<ProfileDTO>("User not found.", 404);

        var usersPosts = await _postRepository.GetUserPostsPagedAsync(profileHeader.UserId, param.PageNumber, param.PageSize, ct);
        var usersPostsCount = await _postRepository.CountUsersPostsAsync(profileHeader.UserId, ct);

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
            _logger.LogError(ex, "Unknown error occured while fetching profile.");
            return new ErrorResultWithData<ProfileDTO>("An error occured while getting user.");
        }
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