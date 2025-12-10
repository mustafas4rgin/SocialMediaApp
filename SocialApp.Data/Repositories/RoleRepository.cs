using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SocialApp.Data.Contexts;
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
    public async Task<bool> RoleNameCheckAsync(string roleName, CancellationToken ct = default)
    => await _context.Set<Role>().AnyAsync(r => r.Name == roleName, ct);
}