using Microsoft.EntityFrameworkCore;
using SocialApp.Data.Contexts;
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
    public IQueryable<Comment> GetPostComments(int postId, CancellationToken ct = default)
    => _context.Set<Comment>().AsNoTracking().Where(c => !c.IsDeleted && c.PostId == postId);
}