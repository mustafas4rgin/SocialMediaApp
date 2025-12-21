using SocialApp.Domain.DTOs;
using SocialApp.Domain.Entities;

namespace SocialApp.Domain.Contracts;

public interface IUserRepository : IGenericRepository<User>
{
    Task<List<User>> GetAllUsersAsync(string? include, CancellationToken ct = default);
    Task<User?> GetUserByIdAsync(int id, string? include, CancellationToken ct = default);
    Task<ProfileHeaderDTO?> GetProfileHeaderAsync(int userId, CancellationToken ct = default);
    Task<List<UserRecommendationDto>> GetRecommendedUsersAsync(
    int userId,
    int pageNumber,
    int pageSize,
    CancellationToken ct = default);
}