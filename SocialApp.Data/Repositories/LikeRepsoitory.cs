using Microsoft.EntityFrameworkCore;
using SocialApp.Data.Contexts;
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
    {
        var query = _context.Likes
                        .Where(l => l.UserId == userId);

        return await query.AsNoTracking().ToListAsync(ct);
    }
    public async Task<Like?> GetLikeByIdAsync(int id, CancellationToken ct = default)
    {
        var query = _context.Likes
                        .Where(l => l.Id == id);

        return await query.FirstOrDefaultAsync(ct);
    }
    public async Task<List<Like>> GetAllLikesAsync(CancellationToken ct = default)
    {
        var query = _context.Likes;
        
        return await query.AsNoTracking().ToListAsync(ct);
    }
    public async Task<Like?> GetExistLikeAsync(int postId, int userId, CancellationToken ct = default)
    => await _context.Set<Like>().AsNoTracking().FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId, ct);
}
