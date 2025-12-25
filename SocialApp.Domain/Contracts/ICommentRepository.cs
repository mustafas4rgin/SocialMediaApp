using SocialApp.Domain.Entities;

namespace SocialApp.Domain.Contracts;

public interface ICommentRepository : IGenericRepository<Comment>
{
    Task<List<Comment>> GetPostCommentsByPostIdAsync(int postId, CancellationToken ct = default);
}
