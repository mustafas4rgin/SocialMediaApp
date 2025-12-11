using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
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
    public async Task<List<Like>> GetLikesByUserIdAsync(int userId, string? include, CancellationToken ct = default)
    {
        var query = _context.Likes
                        .Where(l => !l.IsDeleted && l.UserId == userId);
        
        if (!string.IsNullOrEmpty(include))
            query = QueryHelper.ApplyIncludesForLike(query, include);

        return await query.AsNoTracking().ToListAsync(ct);
    }
    public async Task<Like?> GetLikeByIdAsync(int id, string? include, CancellationToken ct = default)
    {
        var query = _context.Likes
                        .Where(l => !l.IsDeleted && l.Id == id);

        if (!string.IsNullOrWhiteSpace(include))
            query = QueryHelper.ApplyIncludesForLike(query, include);
        
        return await query.FirstOrDefaultAsync(ct);
    }
    public async Task<List<Like>> GetAllLikesAsync(string? include, CancellationToken ct = default)
    {
        var query = _context.Likes
                        .Where(l => !l.IsDeleted);
        
        if (!string.IsNullOrWhiteSpace(include))
            query = QueryHelper.ApplyIncludesForLike(query, include);
        
        return await query.AsNoTracking().ToListAsync(ct);
    }
    public async Task<Like?> GetExistLikeAsync(int postId, int userId, CancellationToken ct = default)
    => await _context.Set<Like>().AsNoTracking().FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId, ct);
}