using SocialApp.Data.Contexts;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;

namespace SocialApp.Data.Repositories;

public class CommentResponseRepository : GenericRepository<CommentResponse>, ICommentResponseRepository
{
    public CommentResponseRepository(
    AppDbContext context
    ) : base(context)
    {}
}