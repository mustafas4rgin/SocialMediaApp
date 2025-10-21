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
}