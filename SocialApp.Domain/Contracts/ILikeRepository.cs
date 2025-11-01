using SocialApp.Domain.Entities;

namespace SocialApp.Domain.Contracts;

public interface ILikeRepository : IGenericRepository<Like>
{
    IQueryable<Like> GetAllByUserId(int userId, CancellationToken ct = default);
    Task<Like?> GetExistLikeAsync(int postId, int userId, CancellationToken ct = default);
}