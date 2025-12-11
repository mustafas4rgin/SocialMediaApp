using SocialApp.Domain.Entities;

namespace SocialApp.Domain.Contracts;

public interface ILikeRepository : IGenericRepository<Like>
{
    Task<List<Like>> GetLikesByUserIdAsync(int userId, string? include, CancellationToken ct = default);
    Task<List<Like>> GetAllLikesAsync(string? include, CancellationToken ct = default);
    Task<Like?> GetLikeByIdAsync(int id, string? include, CancellationToken ct = default);
    Task<Like?> GetExistLikeAsync(int postId, int userId, CancellationToken ct = default);
}