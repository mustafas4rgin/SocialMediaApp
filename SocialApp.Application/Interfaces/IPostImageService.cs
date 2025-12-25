using SocialApp.Domain.Contracts;
using SocialApp.Domain.DTOs.List;
using SocialApp.Domain.Entities;

namespace SocialApp.Application.Interfaces;

public interface IPostImageService : IGenericService<PostImage>
{
    Task<IServiceResultWithData<List<PostImageDTO>>> GetPostImages(int postId, CancellationToken ct = default);
}