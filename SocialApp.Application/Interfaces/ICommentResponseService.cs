using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;
using SocialApp.Domain.Parameters;

namespace SocialApp.Application.Interfaces;

public interface ICommentResponseService : IGenericService<CommentResponse>
{
    Task<IServiceResultWithData<CommentResponse>> GetCommentResponseByIdWithIncludesAsync(int id, QueryParameters parma, CancellationToken ct = default);
    Task<IServiceResultWithData<IEnumerable<CommentResponse>>> GetAllCommentResponsesWithIncludesAsync(QueryParameters param, CancellationToken ct = default);
}