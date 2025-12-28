using SocialApp.Domain.Contracts;
using SocialApp.Domain.DTOs;
using SocialApp.Domain.DTOs.List;
using SocialApp.Domain.Entities;
using SocialApp.Domain.Parameters;

namespace SocialApp.Application.Interfaces;

public interface IUserService : IGenericService<User>
{
    Task<IServiceResultWithData<List<UserRecommendationDto>>> GetRecommendedUsersAsync(int userId, int pageNumber,int pageSize, CancellationToken ct = default);
    Task<IServiceResultWithData<UserDTO>> GetUserInfoByUserNameAsync(string userName, CancellationToken ct = default);
}