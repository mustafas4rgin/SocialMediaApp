using SocialApp.Domain.Contracts;
using SocialApp.Domain.DTOs;
using SocialApp.Domain.Entities;
using SocialApp.Domain.Parameters;

namespace SocialApp.Application.Interfaces;

public interface IPostService : IGenericService<Post>
{
    Task<IServiceResultWithData<Post>> GetPostByIdWithIncludesAsync(
        int id,
        QueryParameters param,
        CancellationToken ct);
    Task<IServiceResultWithData<IEnumerable<Post>>> GetAllPostsWithIncludesAsync(QueryParameters param, CancellationToken ct);
    Task<IServiceResultWithData<List<FeedDTO>>> GetFeedAsync(
    int userId,
    int pageNumber,
    int pageSize,
    CancellationToken ct = default);
}