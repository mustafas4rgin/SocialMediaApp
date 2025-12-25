
using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;
using SocialApp.Domain.Parameters;

namespace SocialApp.Application.Interfaces;

public interface ILikeService : IGenericService<Like>
{
    Task<IServiceResultWithData<IEnumerable<Like>>> GetAllLikesCachedAsync(QueryParameters param, CancellationToken ct);
    Task<IServiceResultWithData<IEnumerable<Like>>> GetLikesByUserIdAsync(int userId, QueryParameters param, CancellationToken ct = default);
}