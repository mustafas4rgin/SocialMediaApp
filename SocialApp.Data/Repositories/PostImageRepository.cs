using Microsoft.EntityFrameworkCore;
using SocialApp.Data.Contexts;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;

namespace SocialApp.Data.Repositories;

public class PostImageRepository : GenericRepository<PostImage>, IPostImageRepository
{
    private readonly AppDbContext _context;
    public PostImageRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
    public async Task<List<PostImage>> GetPostImages(int postId, CancellationToken ct = default)
    {
        return await _context.PostImages
                        .AsNoTracking()
                        .Where(p => !p.IsDeleted && p.PostId == postId)
                        .OrderByDescending(p => p.CreatedAt)
                        .ThenByDescending(p => p.Id)
                        .ToListAsync(ct);
                        
    }
}