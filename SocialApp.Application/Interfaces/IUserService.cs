using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;
using SocialApp.Domain.Parameters;

namespace SocialApp.Application.Interfaces;

public interface IUserService : IGenericService<User>
{
    Task<IServiceResultWithData<IEnumerable<User>>> GetAllUsersWithIncludesAsync(QueryParameters param, CancellationToken ct);
    Task<IServiceResultWithData<User>> GetUserByIdWithIncludesAsync(int id, QueryParameters param, CancellationToken ct = default);
}