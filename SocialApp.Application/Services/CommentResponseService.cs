
using FluentValidation;
using Microsoft.Extensions.Logging;
using SocialApp.Application.Interfaces;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;
using SocialApp.Domain.Parameters;
using SocialApp.Domain.Results.Error;
using SocialApp.Domain.Results.Success;

namespace SocialApp.Application.Services.CommentResponseService;

public class CommentResponseService : GenericService<CommentResponse>, ICommentResponseService
{
    private readonly ICommentResponseRepository _commentResponseRepository;
    private readonly ILogger<CommentResponseService> _logger;
    public CommentResponseService(
    IValidator<CommentResponse> validator,
    ICommentResponseRepository commentResponseRepository,
    ILogger<CommentResponseService> logger
    ) : base(validator, commentResponseRepository, logger)
    {
        _logger = logger;
        _commentResponseRepository = commentResponseRepository;
    }
    public async Task<IServiceResultWithData<IEnumerable<CommentResponse>>> GetResponsesByCommentId(
    int commentId,
    QueryParameters param,
    CancellationToken ct = default)
    {
        try
        {
            var responses = await _commentResponseRepository.GetResponsesByCommentIdAsync(commentId, ct);
            
            if (!responses.Any())
                return new ErrorResultWithData<IEnumerable<CommentResponse>>("There is no response.", 404);

            return new SuccessResultWithData<IEnumerable<CommentResponse>>("Responses: ", responses);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while getting responses.");
            return new ErrorResultWithData<IEnumerable<CommentResponse>>("An unexpected error occured.");
        }
    }

    public async Task<IServiceResultWithData<IEnumerable<CommentResponse>>> GetAllCommentResponsesWithIncludesAsync(QueryParameters param, CancellationToken ct = default)
    {
        try
        {
            var commentResponses = await _commentResponseRepository.GetAllResponsesAsync(ct);

            if (!commentResponses.Any())
                return new ErrorResultWithData<IEnumerable<CommentResponse>>("There is no response.", 404);

            return new SuccessResultWithData<IEnumerable<CommentResponse>>("Response(s) found", commentResponses);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while getting responses.");
            return new ErrorResultWithData<IEnumerable<CommentResponse>>("An unexpected error occured.");
        }
    }
    public async Task<IServiceResultWithData<CommentResponse>> GetCommentResponseByIdWithIncludesAsync(int id, QueryParameters parma, CancellationToken ct = default)
    {
        try
        {
            var commentResponse = await _commentResponseRepository.GetCommentResponseByIdAsync(id, ct);

            if (commentResponse is null)
                return new ErrorResultWithData<CommentResponse>($"There is no response with ID : {id}", 404);

            return new SuccessResultWithData<CommentResponse>("Response found.", commentResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occured while getting response with ID : {id}");
            return new ErrorResultWithData<CommentResponse>("An unexpected error occured.");
        }
    }
}
