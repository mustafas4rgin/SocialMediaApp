using SocialApp.Domain.Entities;

namespace SocialApp.Domain.Contracts;

public interface ICommentRepository : IGenericRepository<Comment>
{
    IQueryable<Comment> GetPostComments(int postId, CancellationToken ct = default);
}