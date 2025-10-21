using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;
using SocialApp.Domain.Parameters;

namespace SocialApp.Application.Interfaces;

public interface IRoleService : IGenericService<Role>
{
    Task<IServiceResultWithData<IEnumerable<Role>>> GetRolesWithIncludesAsync(QueryParameters param, CancellationToken ct = default);
    Task<IServiceResultWithData<Role>> GetRoleByIdWithIncludesAsync(int id, QueryParameters param, CancellationToken ct = default);
}