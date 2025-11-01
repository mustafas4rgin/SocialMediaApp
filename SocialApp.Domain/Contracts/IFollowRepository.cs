using SocialApp.Domain.Entities;

namespace SocialApp.Domain.Contracts;

public interface IFollowRepository : IGenericRepository<Follow>
{
    Task<Follow?> GetExistFollowAsync(int followerId, int followingId, CancellationToken ct);
    IQueryable<Follow> GetFollowsByFollowingId(int followingId, CancellationToken ct);
}