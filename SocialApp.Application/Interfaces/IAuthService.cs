using SocialApp.Domain.Contracts;
using SocialApp.Domain.DTOs.Auth;

namespace SocialApp.Application.Interfaces;

public interface IAuthService
{
    Task<IServiceResultWithData<TokenResponseDTO>> GenerateAccessTokenWithRefreshTokenAsync(RefreshTokenRequestDTO dto, CancellationToken ct);
    Task<IServiceResultWithData<TokenResponseDTO>> LoginAsync(LoginDTO dto, CancellationToken ct);
}