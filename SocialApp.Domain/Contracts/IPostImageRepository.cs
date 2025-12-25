using SocialApp.Domain.Entities;

namespace SocialApp.Domain.Contracts;

public interface IPostImageRepository : IGenericRepository<PostImage>
{
    Task<List<PostImage>> GetPostImages(int postId, CancellationToken ct = default);
}