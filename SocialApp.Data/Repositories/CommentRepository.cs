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
    public async Task<List<Comment>> GetPostCommentsByPostIdAsync(int postId, CancellationToken ct = default)
    => await Query(includeDeleted: false, asNoTracking: true)
                .Where(c => c.PostId == postId)
                .OrderedByNewest()
                .ToListAsync(ct);
                    
}
