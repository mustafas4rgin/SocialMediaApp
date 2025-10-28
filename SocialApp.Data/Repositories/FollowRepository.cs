using Microsoft.EntityFrameworkCore;
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
    public async Task<Follow?> GetExistFollowAsync(int followerId, int followingId, CancellationToken ct = default)
    => await _context.Set<Follow>().AsNoTracking().FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowingId == followingId, ct);
}