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
    public async Task<List<Follow>> GetUsersFollowings(int userId, CancellationToken ct = default)
    {
        var query = _context.Follows
                        .Where(f => f.FollowingId == userId);
                
        return await query.AsNoTracking().ToListAsync(ct);
    }
    public async Task<Follow?> GetFollowByIdAsync(int id, CancellationToken ct = default)
    {
        var query = _context.Follows
                        .Where(f => f.Id == id);

        return await query.AsNoTracking().FirstOrDefaultAsync(ct);
    }
    public async Task<List<Follow>> GetFollowsByFollowingIdAsync(int followingId, CancellationToken ct = default)
    {
        var query = _context.Follows
                        .Where(f => f.FollowingId == followingId);
        
        return await query.AsNoTracking().ToListAsync(ct);
    }
    public async Task<List<Follow>> GetAllFollowsAsync(CancellationToken ct = default)
    {
        var query = _context.Follows;
        
        return await query.AsNoTracking().ToListAsync(ct);
    }
    public async Task<Follow?> GetExistFollowAsync(int followerId, int followingId, CancellationToken ct = default)
    => await _context.Set<Follow>().AsNoTracking().FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowingId == followingId, ct);
}
