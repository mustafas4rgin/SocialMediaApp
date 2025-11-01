using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;
using SocialApp.Domain.Parameters;

namespace SocialApp.Application.Interfaces;

public interface IFollowService : IGenericService<Follow>
{
    Task<IServiceResultWithData<IEnumerable<Follow>>> GetFollowsByFollowingId(int FollowingId, QueryParameters param, CancellationToken ct = default);
    Task<IServiceResultWithData<IEnumerable<Follow>>> GetAllFollowsWithIncludesAsync(QueryParameters param, CancellationToken ct);
    Task<IServiceResultWithData<Follow>> GetFollowByIdWithIncludesAsync(int id, QueryParameters param, CancellationToken ct);
}