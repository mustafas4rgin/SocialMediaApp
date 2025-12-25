using FluentValidation;
using Microsoft.Extensions.Logging;
using SocialApp.Application.Interfaces;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.DTOs.List;
using SocialApp.Domain.Entities;
using SocialApp.Domain.Results.Error;
using SocialApp.Domain.Results.Success;

namespace SocialApp.Application.Services;

public class PostImageService : GenericService<PostImage>, IPostImageService
{
    private readonly IPostImageRepository _postImageRepository;
    private readonly IPostRepository _postRepository;
    ILogger<PostImageService> _logger;
    public PostImageService(
    IPostRepository postRepository,
    IValidator<PostImage> validator,
    IPostImageRepository postImageRepository,
    ILogger<PostImageService> logger
    ) : base(validator, postImageRepository, logger)
    {
        _postImageRepository = postImageRepository;
        _logger = logger;
        _postRepository = postRepository;
    }
    public async Task<IServiceResultWithData<List<PostImageDTO>>> GetPostImages(int postId, CancellationToken ct = default)
    {
        var post = await _postRepository.GetByIdAsync(id: postId, includeDeleted: false, ct: ct);

        if (post is null)
            return new ErrorResultWithData<List<PostImageDTO>>($"There is no post with ID : {postId}.", 404);

        try
        {
            var images = await _postImageRepository.GetPostImages(postId, ct);

            if (!images.Any())
                return new ErrorResultWithData<List<PostImageDTO>>("There is no image for this post.");

            var dto = images.Select(p => new PostImageDTO
            {
                File = p.File,
                PostId = p.PostId
            }).ToList();

            return new SuccessResultWithData<List<PostImageDTO>>("Images found.", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while getting post images.");
            return new ErrorResultWithData<List<PostImageDTO>>("An unknown error occured while getting post images.");
        }
    }
}