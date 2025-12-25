using FluentValidation;
using Microsoft.Extensions.Logging;
using SocialApp.Application.Interfaces;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.DTOs.List;
using SocialApp.Domain.Entities;
using SocialApp.Domain.Results.Error;
using SocialApp.Domain.Results.Success;

namespace SocialApp.Application.Services;

public class PostBrutalService : GenericService<PostBrutal>, IPostBrutalService
{
    private readonly IPostBrutalRepository _postBrutalRepository;
    private readonly IPostRepository _postRepository;
    private readonly ILogger<PostBrutalService> _logger;
    public PostBrutalService(
    IValidator<PostBrutal> validator,
    IPostBrutalRepository postBrutalRepository,
    IPostRepository postRepository,
    ILogger<PostBrutalService> logger
    ) : base(validator, postBrutalRepository, logger)
    {
        _logger = logger;
        _postRepository = postRepository;
        _postBrutalRepository = postBrutalRepository;
    }
    public async Task<IServiceResultWithData<List<PostBrutalDTO>>> GetPostBrutalByPostIdAsync(int postId, CancellationToken ct = default)
    {
        var post = await _postRepository.GetByIdAsync(postId, ct);

        if (post is null)
            return new ErrorResultWithData<List<PostBrutalDTO>>($"There is no post with ID : {postId}.", 404);

        try
        {
            var postBrutals = await _postBrutalRepository.GetPostBrutalsByPostId(postId, ct);

            if (!postBrutals.Any())
                return new ErrorResultWithData<List<PostBrutalDTO>>("There is no brutal for that post.");
            
            var dto = postBrutals.Select(pb => new PostBrutalDTO
            {
                File = pb.File,
                PostId = pb.PostId
            }).ToList();

            return new SuccessResultWithData<List<PostBrutalDTO>>("Brutals found.", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unknown error occured while getting brutals.");
            return new ErrorResultWithData<List<PostBrutalDTO>>("An error occured while getting brutals.");
        }
    }
}