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
    public async Task<Follow?> GetFollowByIdAsync(int id, string? include, CancellationToken ct = default)
    {
        var query = _context.Follows
                        .Where(f => !f.IsDeleted && f.Id == id);
        
        if (!string.IsNullOrWhiteSpace(include))
            query = QueryHelper.ApplyIncludesForFollow(query, include);

        return await query.FirstOrDefaultAsync(ct);
    }
    public async Task<List<Follow>> GetFollowsByFollowingIdAsync(int followingId, string? include, CancellationToken ct = default)
    {
        var query = _context.Follows
                        .Where(f => !f.IsDeleted && f.FollowingId == followingId);
        
        if (!string.IsNullOrWhiteSpace(include))
            query = QueryHelper.ApplyIncludesForFollow(query, include);
        
        return await query.ToListAsync(ct);
    }
    public async Task<List<Follow>> GetAllFollowsAsync(string? include, CancellationToken ct = default)
    {
        var query = _context.Follows
                        .Where(f => !f.IsDeleted);
        
        if (!string.IsNullOrWhiteSpace(include))
            query = QueryHelper.ApplyIncludesForFollow(query, include);
        
        return await query.ToListAsync(ct);
    }
    public async Task<Follow?> GetExistFollowAsync(int followerId, int followingId, CancellationToken ct = default)
    => await _context.Set<Follow>().AsNoTracking().FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowingId == followingId, ct);
}