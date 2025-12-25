using SocialApp.Domain.Entities;

namespace SocialApp.Domain.Contracts;

public interface IFollowRepository : IGenericRepository<Follow>
{
    Task<Follow?> GetExistFollowAsync(int followerId, int followingId, CancellationToken ct);
    Task<List<Follow>> GetAllFollowsAsync(CancellationToken ct = default);
    Task<Follow?> GetFollowByIdAsync(int id, CancellationToken ct = default);
    Task<List<Follow>> GetFollowsByFollowingIdAsync(int followingId, CancellationToken ct = default);
    Task<List<Follow>> GetUsersFollowings(int userId, CancellationToken ct = default);
}
