
using Microsoft.EntityFrameworkCore;
using SocialApp.Data.Contexts;
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
    public async Task<List<CommentResponse>> GetResponsesByCommentIdAsync(int commentId, CancellationToken ct = default)
    {
        var query = _context.Responses
                        .Where(cr => cr.CommentId == commentId);

        return await query.AsNoTracking().ToListAsync(ct);
    }
    public async Task<List<CommentResponse>> GetAllResponsesAsync(CancellationToken ct = default)
    {
        var query = _context.Responses;

        return await query.AsNoTracking().ToListAsync(ct);
    }
    public async Task<CommentResponse?> GetCommentResponseByIdAsync(int id, CancellationToken ct = default)
    {
        var query = _context.Responses;

        return await query.FirstOrDefaultAsync(cr => cr.Id == id, ct);
    }

}
