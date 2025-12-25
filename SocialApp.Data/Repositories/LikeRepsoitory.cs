using Microsoft.EntityFrameworkCore;
using SocialApp.Data.Contexts;
using SocialApp.Data.Helpers;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;

namespace SocialApp.Data.Repositories;

public class LikeRepository : GenericRepository<Like>, ILikeRepository
{
    private readonly AppDbContext _context;
    public LikeRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
    public async Task<List<Like>> GetLikesByUserIdAsync(int userId, CancellationToken ct = default)
    => await Query(includeDeleted: false, asNoTracking: true)
                    .Where(l => l.UserId == userId)
                    .OrderedByNewest()
                    .ToListAsync(ct);
    public async Task<Like?> GetExistLikeAsync(int postId, int userId, CancellationToken ct = default)
    => await _context.Set<Like>().AsNoTracking().FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId, ct);
}
