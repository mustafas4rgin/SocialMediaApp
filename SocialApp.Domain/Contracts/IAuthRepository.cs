
using SocialApp.Domain.Entities;

namespace SocialApp.Domain.Contracts;
public interface IAuthRepository
{
    IQueryable<User> GetUsersWithRoles(CancellationToken ct = default);
    IQueryable<AccessToken> GetAccessTokens(CancellationToken ct = default);
    IQueryable<RefreshToken> GetRefreshTokens(CancellationToken ct = default);

    Task<User?> GetUserByIdWithRoleAsync(int userId, CancellationToken ct = default);
    Task<RefreshToken?> GetRefreshTokenByTokenAsync(string token, CancellationToken ct = default);
    Task<AccessToken?> GetAccessTokenByTokenAsync(string token, CancellationToken ct = default);
    Task<AccessToken?> GetAccessTokenByRefreshTokenAsync(string refreshToken, CancellationToken ct = default);
    Task AddAccessTokenAsync(AccessToken accessToken);
    Task AddRefreshTokenAsync(RefreshToken refreshToken);
    void DeleteRefreshToken(RefreshToken refreshToken, CancellationToken ct = default);
    void DeleteAccessToken(AccessToken accessToken, CancellationToken ct = default);
    Task<User?> GetUserByEmailWithRoleAsync(string email, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct);
    void UpdateAccessToken(AccessToken accessToken);
    void UpdateRefreshToken(RefreshToken refreshToken);
    Task<AccessToken?> GetAccessTokenByUserIdAsync(int userId, CancellationToken ct = default);
    Task<bool> UserExistsAsync(string userName = "", string email = "", CancellationToken ct = default);
    Task RegisterUserAsync(User user, CancellationToken ct = default);
    Task<List<AccessToken>> GetExpiredAccessTokensAsync(CancellationToken ct = default);
    void Delete(AccessToken token);
}