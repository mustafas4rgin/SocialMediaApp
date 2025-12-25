using SocialApp.Domain.Entities;

namespace SocialApp.Domain.Contracts;

public interface ICommentRepository : IGenericRepository<Comment>
{
    Task<List<Comment>> GetPostCommentsByPostIdAsync(int postId, CancellationToken ct = default);
    Task<List<Comment>> GetPostCommentsAsync(CancellationToken ct = default);
    Task<Comment?> GetPostCommentByIdAsync(int id, CancellationToken ct = default);
}
