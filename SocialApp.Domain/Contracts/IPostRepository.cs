using SocialApp.Domain.Entities;

namespace SocialApp.Domain.Contracts;

public interface IPostRepository : IGenericRepository<Post>
{
    Task<List<Post>> GetAllPostsAsync(string? include, CancellationToken ct = default);
    Task<Post?> GetPostByIdAsync(int id, string? include, CancellationToken ct = default);
}