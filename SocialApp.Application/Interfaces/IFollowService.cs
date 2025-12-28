using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;
using SocialApp.Domain.Parameters;

namespace SocialApp.Application.Interfaces;

public interface IFollowService : IGenericService<Follow>
{
    Task<IServiceResult> DeleteFollowAsync(int followId, CancellationToken ct = default);
    Task<IServiceResultWithData<IEnumerable<Follow>>> GetFollowsByFollowingId(int FollowingId, QueryParameters param, CancellationToken ct = default);
}