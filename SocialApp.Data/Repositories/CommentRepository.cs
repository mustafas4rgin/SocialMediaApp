using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SocialApp.Data.Contexts;
using SocialApp.Data.Helpers;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;

namespace SocialApp.Data.Repositories;

public class CommentRepository : GenericRepository<Comment>, ICommentRepository
{
    private readonly AppDbContext _context;
    public CommentRepository(
    AppDbContext context
    ) : base(context)
    {
        _context = context;
    }
    public async Task<List<Comment>> GetPostCommentsByPostIdAsync(int postId, string? include, CancellationToken ct = default)
    {
        var query = _context.Comments
                        .Where(c => !c.IsDeleted && c.PostId == postId);
        
        if (!string.IsNullOrWhiteSpace(include))
            query = QueryHelper.ApplyIncludesForComment(query, include);

        return await query.AsNoTracking().ToListAsync(ct);
    }
    public async Task<List<Comment>> GetPostCommentsAsync(string? include, CancellationToken ct = default)
    {
        var query = _context.Comments
                        .Where(c => !c.IsDeleted);
        
        if (!string.IsNullOrWhiteSpace(include))
            query = QueryHelper.ApplyIncludesForComment(query, include);
        
        return await query.AsNoTracking().ToListAsync();
    }
    public async Task<Comment?> GetPostCommentByIdAsync(int id, string? include, CancellationToken ct = default)
    {
        var query = _context.Comments
                        .Where(c => !c.IsDeleted && c.Id == id);
        
        if (!string.IsNullOrWhiteSpace(include))
            query = QueryHelper.ApplyIncludesForComment(query, include);
        
        return await query.AsNoTracking().FirstOrDefaultAsync(ct);
    }
}