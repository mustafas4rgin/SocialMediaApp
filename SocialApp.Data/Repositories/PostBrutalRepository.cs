using Microsoft.EntityFrameworkCore;
using SocialApp.Data.Contexts;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;

namespace SocialApp.Data.Repositories;

public class PostBrutalRepository : GenericRepository<PostBrutal>, IPostBrutalRepository
{
    private readonly AppDbContext _context;
    public PostBrutalRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
    public async Task<List<PostBrutal>> GetPostBrutalsByPostId(int postId, CancellationToken ct = default)
    {
        return await _context.Brutals
                        .AsNoTracking()
                        .Where(pb => pb.PostId == postId)
                        .OrderByDescending(pb => pb.CreatedAt)
                        .ThenByDescending(pb => pb.Id)
                        .ToListAsync(ct);
    }
}
