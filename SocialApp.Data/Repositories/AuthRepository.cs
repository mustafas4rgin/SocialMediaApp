using Microsoft.EntityFrameworkCore;
using SocialApp.Data.Contexts;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;
namespace SocialApp.Data.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly AppDbContext _context;

    public AuthRepository(AppDbContext context)
    {
        _context = context;
    }

    public IQueryable<User> GetUsersWithRoles(CancellationToken ct = default)
    => _context.Set<User>().Include(u => u.Role).AsNoTracking();

    public IQueryable<AccessToken> GetAccessTokens(CancellationToken ct = default)
    => _context.Set<AccessToken>().AsNoTracking();

    public IQueryable<RefreshToken> GetRefreshTokens(CancellationToken ct = default)
    => _context.Set<RefreshToken>().AsNoTracking();

    public async Task<RefreshToken?> GetRefreshTokenByTokenAsync(string token, CancellationToken ct = default)
    => await _context.Set<RefreshToken>()
    .FirstOrDefaultAsync(rt => rt.Token == token, ct);
    public async Task<AccessToken?> GetAccessTokenByTokenAsync(string token, CancellationToken ct = default)
    => await _context.Set<AccessToken>()
    .FirstOrDefaultAsync(at => at.Token == token, ct);
    public async Task<User?> GetUserByEmailWithRoleAsync(string email, CancellationToken ct = default)
    => await _context.Set<User>()
    .Include(u => u.Role)
    .FirstOrDefaultAsync(u => u.Email == email);
    public async Task<User?> GetUserByIdWithRoleAsync(int userId, CancellationToken ct = default)
    => await _context.Set<User>()
    .Include(u => u.Role)
    .FirstOrDefaultAsync(u => u.Id == userId, ct);

    public void UpdateAccessToken(AccessToken accessToken)
    {
        if (accessToken is null) return;
        _context.Update(accessToken);
    }
    public void UpdateRefreshToken(RefreshToken refreshToken)
    {
        if (refreshToken is null) return;
        _context.Update(refreshToken);
    }
    public async Task AddAccessTokenAsync(AccessToken accessToken)
    {
        if (accessToken is null) return;
        await _context.AddAsync(accessToken);
    }
    public async Task<AccessToken?> GetAccessTokenByUserIdAsync(int userId, CancellationToken ct = default)
    {
        return await _context.AccessTokens
            .OrderByDescending(x => x.ExpiresAt)
            .FirstOrDefaultAsync(x => x.UserId == userId, ct);
    }
    public async Task<AccessToken?> GetAccessTokenByRefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    => await _context.Set<AccessToken>()
    .FirstOrDefaultAsync(at => at.RefreshToken == refreshToken, ct);

    public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
    {
        if (refreshToken is null) return;
        await _context.AddAsync(refreshToken);
    }

    public void DeleteRefreshToken(RefreshToken refreshToken, CancellationToken ct = default)
    {
        if (refreshToken is null) return;
        _context.Set<RefreshToken>().Remove(refreshToken);
    }

    public void DeleteAccessToken(AccessToken accessToken, CancellationToken ct = default)
    {
        if (accessToken is null) return;
        _context.Set<AccessToken>().Remove(accessToken);
    }
    public async Task SaveChangesAsync(CancellationToken ct)
    => await _context.SaveChangesAsync(ct);

    public async Task<bool> UserExistsAsync(string userName = "", string email = "", CancellationToken ct = default) =>
    await _context.Set<User>().AnyAsync(u => u.UserName == userName || u.Email == email, ct);
    public async Task RegisterUserAsync(User user, CancellationToken ct = default) =>
    await _context.Set<User>().AddAsync(user, ct);
}