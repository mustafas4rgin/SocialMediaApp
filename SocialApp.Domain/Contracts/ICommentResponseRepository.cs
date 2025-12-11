using SocialApp.Domain.Entities;

namespace SocialApp.Domain.Contracts;

public interface ICommentResponseRepository : IGenericRepository<CommentResponse>
{
    Task<List<CommentResponse>> GetResponsesByCommentIdAsync(int commentId, string? include, CancellationToken ct = default);
    Task<List<CommentResponse>> GetAllResponsesAsync(string? include, CancellationToken ct = default);
    Task<CommentResponse?> GetCommentResponseByIdAsync(int id, string? include, CancellationToken ct = default);

}