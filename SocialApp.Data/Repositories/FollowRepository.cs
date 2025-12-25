using Microsoft.EntityFrameworkCore;
using SocialApp.Data.Contexts;
using SocialApp.Data.Helpers;
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
    public async Task<List<Follow>> GetUsersFollowings(int userId, CancellationToken ct = default)
    => await Query(includeDeleted: false, asNoTracking: true)
                    .Where(f => f.FollowerId == userId)
                    .OrderedByNewest()
                    .ToListAsync(ct);
        
    public async Task<Follow?> GetFollowByIdAsync(int id, CancellationToken ct = default)
    => await Query(includeDeleted: false, asNoTracking: true)
                        .FirstOrDefaultAsync(f => f.Id == id, ct);
    public async Task<List<Follow>> GetFollowsByFollowingIdAsync(int followingId, CancellationToken ct = default)
    => await Query(includeDeleted: false, asNoTracking: true)
                        .OrderedByNewest()
                        .Where(f => f.FollowingId == followingId)
                        .ToListAsync(ct);
    public async Task<Follow?> GetExistFollowAsync(int followerId, int followingId, CancellationToken ct = default)
    => await _context.Follows.AsNoTracking().FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowingId == followingId, ct);
}
