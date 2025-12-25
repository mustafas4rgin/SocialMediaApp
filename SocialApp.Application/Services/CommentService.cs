using System.Security.Cryptography.X509Certificates;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using SocialApp.Application.Helpers;
using SocialApp.Application.Interfaces;
using SocialApp.Application.Validators;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;
using SocialApp.Domain.Parameters;
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
    public CommentService(
    ICommentRepository commentRepository,
    IValidator<Comment> validator,
    ILogger<CommentService> logger,
    IPostRepository postRepository,
    IUserRepository userRepository
    ) : base(validator, commentRepository, logger)
    {
        _userRepository = userRepository;
        _postRepository = postRepository;
        _validator = validator;
        _commentRepository = commentRepository;
        _logger = logger;
    }
    public async Task<IServiceResultWithData<IEnumerable<Comment>>> GetPostCommentsByPostId(int postId,QueryParameters param, CancellationToken ct = default)
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
            _logger.LogError(ex, "An error occured while getting commentsç");
            return new ErrorResultWithData<IEnumerable<Comment>>("An unexpected error occured.");
        }
    }
    public async Task<IServiceResultWithData<IEnumerable<Comment>>> GetAllCommentsWithIncludesAsync(QueryParameters param, CancellationToken ct = default)
    {
        try
        {
            var comments = await _commentRepository.GetPostCommentsAsync(ct);

            if (!comments.Any())
                return new ErrorResultWithData<IEnumerable<Comment>>("There is no comment.", 404);

            return new SuccessResultWithData<IEnumerable<Comment>>("Comments found.", comments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while getting comments.");
            return new ErrorResultWithData<IEnumerable<Comment>>("An unexpected error occured.");
        }
    }
    public async Task<IServiceResultWithData<Comment>> GetCommentByIdWithIncludesAsync(int id, QueryParameters param, CancellationToken ct = default)
    {
        try
        {
            var comment = await _commentRepository.GetPostCommentByIdAsync(id, ct);

            if (comment is null)
                return new ErrorResultWithData<Comment>($"There is no comment with ID : {id}", 404);

            return new SuccessResultWithData<Comment>("Comment found.", comment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occured while getting comment with ID : {id}");
            return new ErrorResultWithData<Comment>("An unexpected error occured.");
        }
    }
    public override async Task<IServiceResultWithData<Comment>> AddAsync(Comment comment, CancellationToken ct = default)
    {
        if (comment is null)
            return new ErrorResultWithData<Comment>("Comment does not exist.");

        var validationResult = await _validator.ValidateAsync(comment, ct);

        if (!validationResult.IsValid)
            return new ErrorResultWithData<Comment>(string.Join(" | ",
                validationResult.Errors.Select(e => e.ErrorMessage)));

        try
        {
            var existingPost = await _postRepository.GetActiveByIdAsync(comment.PostId, ct);

            if (existingPost is null)
                return new ErrorResultWithData<Comment>($"There is no post with ID : {comment.PostId}", 404);

            var existingUser = await _userRepository.GetActiveByIdAsync(comment.UserId, ct);

            if (existingUser is null)
                return new ErrorResultWithData<Comment>($"There is no user with ID : {comment.UserId}", 404);

            await _commentRepository.AddAsync(comment, ct);
            await _commentRepository.SaveChangesAsync(ct);

            return new SuccessResultWithData<Comment>("Comment added successfully.", comment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while adding comment.");
            return new ErrorResultWithData<Comment>(ex.Message);
        }
    }
}
