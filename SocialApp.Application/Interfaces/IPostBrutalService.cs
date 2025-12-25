using SocialApp.Domain.Contracts;
using SocialApp.Domain.DTOs.List;
using SocialApp.Domain.Entities;

namespace SocialApp.Application.Interfaces;

public interface IPostBrutalService : IGenericService<PostBrutal>
{
    Task<IServiceResultWithData<List<PostBrutalDTO>>> GetPostBrutalByPostIdAsync(int postId, CancellationToken ct = default);
}