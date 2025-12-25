using SocialApp.Domain.Entities;

namespace SocialApp.Domain.Contracts;

public interface ICommentResponseRepository : IGenericRepository<CommentResponse>
{
    Task<List<CommentResponse>> GetResponsesByCommentIdAsync(int commentId, CancellationToken ct = default);
    Task<List<CommentResponse>> GetAllResponsesAsync(CancellationToken ct = default);
    Task<CommentResponse?> GetCommentResponseByIdAsync(int id, CancellationToken ct = default);

}
