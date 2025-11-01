using SocialApp.Domain.Entities;

namespace SocialApp.Domain.Contracts;

public interface ICommentResponseRepository : IGenericRepository<CommentResponse>
{
    IQueryable<CommentResponse> GetResponsesByCommentId(int commentId, CancellationToken ct = default);
}