using SocialApp.Application.Interfaces;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;
using SocialApp.Domain.Parameters;

namespace SocialApp.Application.Registrations;

public interface ILikeService : IGenericService<Like>
{
    Task<IServiceResultWithData<IEnumerable<Like>>> GetAllLikesWithIncludesAsync(QueryParameters param, CancellationToken ct);
    Task<IServiceResultWithData<Like>> GetLikeByIdWithIncludesAsync(int id, QueryParameters param, CancellationToken ct);
    Task<IServiceResultWithData<IEnumerable<Like>>> GetLikesByUserIdAsync(int userId, QueryParameters param, CancellationToken ct = default);
}