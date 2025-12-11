using Microsoft.EntityFrameworkCore;
using SocialApp.Data.Contexts;
using SocialApp.Data.Helpers;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;

namespace SocialApp.Data.Repositories;

public class PostRepository : GenericRepository<Post>, IPostRepository
{
    private readonly AppDbContext _context;
    public PostRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
    public async Task<List<Post>> GetAllPostsAsync(string? include, CancellationToken ct = default)
    {
        var query = _context.Posts
                        .Where(p => !p.IsDeleted);
        
        if (!string.IsNullOrWhiteSpace(include))
            query = QueryHelper.ApplyIncludesforPost(query, include);
        
        return await query.AsNoTracking().ToListAsync(ct);
    }
    public async Task<Post?> GetPostByIdAsync(int id, string? include, CancellationToken ct = default)
    {
        var query = _context.Posts
                        .Where(p => !p.IsDeleted && p.Id == id);
        
        if (!string.IsNullOrWhiteSpace(include))
            query = QueryHelper.ApplyIncludesforPost(query, include);
        
        return await query.AsNoTracking().FirstOrDefaultAsync(ct);
    }
}