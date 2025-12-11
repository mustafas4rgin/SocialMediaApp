using SocialApp.Domain.Entities;

namespace SocialApp.Domain.Contracts;

public interface ICommentRepository : IGenericRepository<Comment>
{
    Task<List<Comment>> GetPostCommentsByPostIdAsync(int postId, string? include, CancellationToken ct = default);
    Task<List<Comment>> GetPostCommentsAsync(string? include, CancellationToken ct = default);
    Task<Comment?> GetPostCommentByIdAsync(int id, string? include, CancellationToken ct = default);
}