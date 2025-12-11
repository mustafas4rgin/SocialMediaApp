using SocialApp.Domain.Entities;

namespace SocialApp.Domain.Contracts;

public interface IUserRepository : IGenericRepository<User>
{
    Task<List<User>> GetAllUsersAsync(string? include, CancellationToken ct = default);
    Task<User?> GetUserByIdAsync(int id, string? include, CancellationToken ct = default);
}