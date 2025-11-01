using System.Security.Cryptography.X509Certificates;
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
    public IQueryable<CommentResponse> GetResponsesByCommentId(int commentId, CancellationToken ct = default)
    => _context.Set<CommentResponse>().AsNoTracking().Where(cr => cr.CommentId == commentId);
}