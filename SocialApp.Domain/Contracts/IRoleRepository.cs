using SocialApp.Domain.Entities;

namespace SocialApp.Domain.Contracts;

public interface IRoleRepository : IGenericRepository<Role>
{
    Task<bool> RoleNameCheckAsync(string roleName, CancellationToken ct = default);
    Task<Role?> GetRoleByIdAsync(int id, string? include, CancellationToken ct = default);
    Task<List<Role>> GetAllRolesAsync(string? include, CancellationToken ct = default);
}