using SocialApp.Domain.Entities;

namespace SocialApp.Domain.Contracts;

public interface IFollowRepository : IGenericRepository<Follow>
{
    Task<Follow?> GetExistFollowAsync(int followerId, int followingId, CancellationToken ct);
    Task<List<Follow>> GetAllFollowsAsync(string? include, CancellationToken ct = default);
    Task<Follow?> GetFollowByIdAsync(int id, string? include, CancellationToken ct = default);
    Task<List<Follow>> GetFollowsByFollowingIdAsync(int followingId, string? include, CancellationToken ct = default);
}