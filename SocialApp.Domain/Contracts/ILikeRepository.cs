using SocialApp.Domain.Entities;

namespace SocialApp.Domain.Contracts;

public interface ILikeRepository : IGenericRepository<Like>
{
    Task<Like?> GetExistLikeAsync(int postId, int userId, CancellationToken ct = default);
}