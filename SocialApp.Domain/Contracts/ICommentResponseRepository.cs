using SocialApp.Domain.Entities;

namespace SocialApp.Domain.Contracts;

public interface ICommentResponseRepository : IGenericRepository<CommentResponse>
{
    Task<List<CommentResponse>> GetResponsesByCommentIdAsync(int commentId, CancellationToken ct = default);

}
