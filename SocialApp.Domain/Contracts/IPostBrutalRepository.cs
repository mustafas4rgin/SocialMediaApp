using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;

namespace SocialApp.Domain.Contracts;

public interface IPostBrutalRepository : IGenericRepository<PostBrutal>
{
    Task<List<PostBrutal>> GetPostBrutalsByPostId(int postId, CancellationToken ct = default);
}