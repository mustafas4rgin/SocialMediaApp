using SocialApp.Domain.Entities;

namespace SocialApp.Domain.Contracts;

public interface ILikeRepository : IGenericRepository<Like>
{
    Task<List<Like>> GetLikesByUserIdAsync(int userId, CancellationToken ct = default);
    Task<Like?> GetExistLikeAsync(int postId, int userId, CancellationToken ct = default);
}
