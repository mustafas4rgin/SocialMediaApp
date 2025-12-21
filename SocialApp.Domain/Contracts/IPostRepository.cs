using SocialApp.Domain.DTOs.List;
using SocialApp.Domain.Entities;

namespace SocialApp.Domain.Contracts;

public interface IPostRepository : IGenericRepository<Post>
{
    Task<List<Post>> GetAllPostsAsync(string? include, CancellationToken ct = default);
    Task<Post?> GetPostByIdAsync(int id, string? include, CancellationToken ct = default);
    Task<List<Post>> GetUsersFeedAsync(int userId, int pageNumber, int pageSize, CancellationToken ct = default);
    Task<int> CountUsersPostsAsync(int userId, CancellationToken ct = default);
    Task<List<PostDTO>> GetUserPostsPagedAsync(int userId, int pageNumber, int pageSize, CancellationToken ct = default);
}