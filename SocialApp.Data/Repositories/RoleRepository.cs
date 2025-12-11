using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SocialApp.Data.Contexts;
using SocialApp.Data.Helpers;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;

namespace SocialApp.Data.Repositories;

public class RoleRepository : GenericRepository<Role>, IRoleRepository
{
    private readonly AppDbContext _context;
    public RoleRepository(
    AppDbContext context
    ) : base(context)
    {
        _context = context;
    }
    public async Task<List<Role>> GetAllRolesAsync(string? include, CancellationToken ct = default)
    {
        var query = _context.Roles
                        .Where(p => !p.IsDeleted);
        
        if (!string.IsNullOrWhiteSpace(include))
            query = QueryHelper.ApplyIncludesForRole(query, include);
        
        return await query.AsNoTracking().ToListAsync(ct);
    }
    public async Task<Role?> GetRoleByIdAsync(int id, string? include, CancellationToken ct = default)
    {
        var query = _context.Roles
                        .Where(r => !r.IsDeleted && r.Id == id);
        
        if (!string.IsNullOrWhiteSpace(include))
            query = QueryHelper.ApplyIncludesForRole(query, include);
        
        return await query.AsNoTracking().FirstOrDefaultAsync(ct);
    }
    public async Task<bool> RoleNameCheckAsync(string roleName, CancellationToken ct = default)
    => await _context.Set<Role>().AnyAsync(r => r.Name == roleName, ct);
}