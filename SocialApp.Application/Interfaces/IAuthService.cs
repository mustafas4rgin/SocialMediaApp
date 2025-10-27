using SocialApp.Domain.Contracts;
using SocialApp.Domain.DTOs.Auth;
using SocialApp.Domain.Entities;

namespace SocialApp.Application.Interfaces;

public interface IAuthService
{
    Task<IServiceResultWithData<TokenResponseDTO>> GenerateAccessTokenWithRefreshTokenAsync(RefreshTokenRequestDTO dto, CancellationToken ct);
    Task<IServiceResultWithData<TokenResponseDTO>> LoginAsync(LoginDTO dto, CancellationToken ct);
    Task<IServiceResultWithData<User>> MeAsync(int userId, string accessTokenString, CancellationToken ct = default);
    Task<IServiceResult> LogOutAsync(string accessTokenString, CancellationToken ct = default);

}