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
    public IQueryable<Like> GetAllByUserId(int userId, CancellationToken ct = default)
    => _context.Set<Like>().AsNoTracking().Where(l => l.UserId == userId);
    public async Task<Like?> GetExistLikeAsync(int postId, int userId, CancellationToken ct = default)
    => await _context.Set<Like>().AsNoTracking().FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId, ct);
}