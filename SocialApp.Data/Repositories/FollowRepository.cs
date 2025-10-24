using SocialApp.Data.Contexts;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;

namespace SocialApp.Data.Repositories;

public class FollowRepository : GenericRepository<Follow>, IFollowRepository
{
    private readonly AppDbContext _context;
    public FollowRepository(
    AppDbContext context
    ) : base(context)
    {
        _context = context;
    }
}