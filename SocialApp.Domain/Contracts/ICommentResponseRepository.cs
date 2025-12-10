using SocialApp.Domain.Entities;

namespace SocialApp.Domain.Contracts;

public interface ICommentResponseRepository : IGenericRepository<CommentResponse>
{
    Task<List<CommentResponse>> GetResponsesByCommentId(int commentId, string? include, CancellationToken ct = default);
}