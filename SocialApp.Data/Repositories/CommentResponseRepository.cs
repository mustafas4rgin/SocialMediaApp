
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
    public async Task<List<CommentResponse>> GetResponsesByCommentIdAsync(int commentId, CancellationToken ct = default)
    => await Query(includeDeleted: false, asNoTracking: true)
                .Where(cr => cr.CommentId == commentId)
                .OrderedByNewest()
                .ToListAsync(ct);

}
