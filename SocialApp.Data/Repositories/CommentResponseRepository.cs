
using Microsoft.EntityFrameworkCore;
using SocialApp.Data.Contexts;
using SocialApp.Data.Helpers;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;

namespace SocialApp.Data.Repositories;

public class CommentResponseRepository : GenericRepository<CommentResponse>, ICommentResponseRepository
{
    private readonly AppDbContext _context;
    public CommentResponseRepository(
    AppDbContext context
    ) : base(context)
    {
        _context = context;
    }
    public async Task<List<CommentResponse>> GetResponsesByCommentId(int commentId, string? include, CancellationToken ct = default)
    {
        var query = _context.Responses
                        .Where(cr =>!cr.IsDeleted && cr.CommentId == commentId);
        
        if (!string.IsNullOrWhiteSpace(include))
            query = QueryHelper.ApplyIncludesForCommentResponse(query, include);
        
        return await query.AsNoTracking().ToListAsync(ct);
    }
}