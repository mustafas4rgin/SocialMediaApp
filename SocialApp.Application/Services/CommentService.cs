using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialApp.Application.Interfaces;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;
using SocialApp.Domain.Events;
using SocialApp.Domain.Results.Error;
using SocialApp.Domain.Results.Success;

namespace SocialApp.Application.Services;

public class CommentService : GenericService<Comment>, ICommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly IValidator<Comment> _validator;
    private readonly IPostRepository _postRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<CommentService> _logger;
    private readonly IMediator _mediator;

    public CommentService(
        IMediator mediator,
        ICommentRepository commentRepository,
        IValidator<Comment> validator,
        ILogger<CommentService> logger,
        IPostRepository postRepository,
        IUserRepository userRepository
    ) : base(validator, commentRepository, logger)
    {
        _mediator = mediator;
        _userRepository = userRepository;
        _postRepository = postRepository;
        _validator = validator;
        _commentRepository = commentRepository;
        _logger = logger;
    }

    public async Task<IServiceResultWithData<IEnumerable<Comment>>> GetPostCommentsByPostId(
        int postId,
        CancellationToken ct = default
    )
    {
        try
        {
            var comments = await _commentRepository.GetPostCommentsByPostIdAsync(postId, ct);

            if (comments is null || !comments.Any())
                return new ErrorResultWithData<IEnumerable<Comment>>("No comment found.", 404);

            return new SuccessResultWithData<IEnumerable<Comment>>("Comments found.", comments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while getting comments.");
            return new ErrorResultWithData<IEnumerable<Comment>>("An unexpected error occured.");
        }
    }

    public override async Task<IServiceResultWithData<Comment>> AddAsync(Comment comment, CancellationToken ct = default)
    {
        if (comment is null)
            return new ErrorResultWithData<Comment>("Comment does not exist.");

        var validationResult = await _validator.ValidateAsync(comment, ct);

        if (!validationResult.IsValid)
            return new ErrorResultWithData<Comment>(
                string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage))
            );

        try
        {
            var existingPost = await _postRepository.GetByIdAsync(
                id: comment.PostId,
                includeDeleted: false,
                ct: ct
            );

            if (existingPost is null)
                return new ErrorResultWithData<Comment>($"There is no post with ID : {comment.PostId}", 404);

            var existingUser = await _userRepository.GetByIdAsync(
                id: comment.UserId,
                includeDeleted: false,
                ct: ct
            );

            if (existingUser is null)
                return new ErrorResultWithData<Comment>($"There is no user with ID : {comment.UserId}", 404);

            await _commentRepository.AddAsync(comment, ct);
            await _commentRepository.SaveChangesAsync(ct);

            try
            {
                await _mediator.Publish(
                    new CommentEvent(existingPost.UserId, comment.UserId),
                    ct
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Comment created but CommentEvent publish failed.");
            }

            return new SuccessResultWithData<Comment>("Comment added successfully.", comment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while adding comment.");
            return new ErrorResultWithData<Comment>("An unexpected error occured while adding comment.");
        }
    }
}
