using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;
using SocialApp.Domain.Parameters;

namespace SocialApp.Application.Interfaces;

public interface ICommentResponseService : IGenericService<CommentResponse>
{
    Task<IServiceResultWithData<IEnumerable<CommentResponse>>> GetResponsesByCommentId(int commentId, QueryParameters param, CancellationToken ct = default);
}