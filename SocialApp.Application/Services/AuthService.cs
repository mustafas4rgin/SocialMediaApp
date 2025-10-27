using System.IdentityModel.Tokens.Jwt;
using FluentValidation;
using Microsoft.Extensions.Options;
using Serilog;
using SocialApp.Application.Helpers;
using SocialApp.Application.Interfaces;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.DTOs.Auth;
using SocialApp.Domain.Entities;
using SocialApp.Domain.Parameters;
using SocialApp.Domain.Results.Error;
using SocialApp.Domain.Results.Success;

namespace SocialApp.Application.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly IValidator<LoginDTO> _loginValidator;
    private readonly JwtOptions _jwt;


    public AuthService(IAuthRepository authRepository, IOptions<JwtOptions> jwtOptions, IValidator<LoginDTO> loginValidator)
    {
        _authRepository = authRepository;
        _jwt = jwtOptions.Value;
        _loginValidator = loginValidator;
    }

    public async Task<IServiceResultWithData<TokenResponseDTO>> GenerateAccessTokenWithRefreshTokenAsync(
    RefreshTokenRequestDTO dto, CancellationToken ct = default)
    {
        try
        {
            var refreshToken = await _authRepository.GetRefreshTokenByTokenAsync(dto.Token, ct);
            if (refreshToken is null)
                return new ErrorResultWithData<TokenResponseDTO>("Invalid token.");

            if (refreshToken.ExpiresAt <= DateTime.UtcNow)
            {
                _authRepository.DeleteRefreshToken(refreshToken, ct);
                await _authRepository.SaveChangesAsync(ct);
                return new ErrorResultWithData<TokenResponseDTO>("Token expired.");
            }

            if (refreshToken.IsUsed)
                return new ErrorResultWithData<TokenResponseDTO>("Refresh token is no longer valid.");

            var user = await _authRepository.GetUserByIdWithRoleAsync(refreshToken.UserId, ct);
            if (user is null)
                return new ErrorResultWithData<TokenResponseDTO>("User no longer exists.");

            var jwtToken = TokenHelper.GenerateAccessToken(user, _jwt);
            var accessTokenString = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            var newRefreshToken = TokenHelper.GenerateRefreshToken(user, _jwt);
            refreshToken.IsUsed = true;

            var dbAccessToken = new AccessToken
            {
                Token = accessTokenString,
                ExpiresAt = jwtToken.ValidTo,
                RefreshToken = newRefreshToken.Token,
                UserId = user.Id
            };

            await _authRepository.AddAccessTokenAsync(dbAccessToken);
            await _authRepository.AddRefreshTokenAsync(newRefreshToken);
            await _authRepository.SaveChangesAsync(ct);

            var result = new TokenResponseDTO
            {
                AccessToken = dbAccessToken.Token,
                AccessTokenExpiresAt = dbAccessToken.ExpiresAt,
                RefreshToken = newRefreshToken.Token,
                RefreshTokenExpiresAt = newRefreshToken.ExpiresAt
            };

            return new SuccessResultWithData<TokenResponseDTO>("Tokens refreshed.", result);
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return new ErrorResultWithData<TokenResponseDTO>(ex.Message);
        }


    }
    public async Task<IServiceResultWithData<TokenResponseDTO>> LoginAsync(LoginDTO dto, CancellationToken ct = default)
    {
        try
        {
            var validationResult = await _loginValidator.ValidateAsync(dto, ct);

            if (!validationResult.IsValid)
                return new ErrorResultWithData<TokenResponseDTO>(string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage)));

            var user = await _authRepository.GetUserByEmailWithRoleAsync(dto.Email, ct);

            if (user is null || user.IsDeleted)
                return new ErrorResultWithData<TokenResponseDTO>("Auth failed.");

            if (!HashingHelper.VerifyPasswordHash(dto.Password, user.PasswordHash, user.PasswordSalt))
                return new ErrorResultWithData<TokenResponseDTO>("Auth failed.");


            var accessJwtToken = TokenHelper.GenerateAccessToken(user, _jwt);

            var refreshToken = TokenHelper.GenerateRefreshToken(user, _jwt);

            var accessToken = new AccessToken
            {
                Token = new JwtSecurityTokenHandler().WriteToken(accessJwtToken),
                ExpiresAt = accessJwtToken.ValidTo,
                UserId = user.Id,
                RefreshToken = refreshToken.Token
            };

            var response = new TokenResponseDTO
            {
                AccessToken = accessToken.Token,
                AccessTokenExpiresAt = accessToken.ExpiresAt,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiresAt = refreshToken.ExpiresAt
            };

            await _authRepository.AddRefreshTokenAsync(refreshToken);
            await _authRepository.AddAccessTokenAsync(accessToken);
            await _authRepository.SaveChangesAsync(ct);

            return new SuccessResultWithData<TokenResponseDTO>("Login successful.", response);
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return new ErrorResultWithData<TokenResponseDTO>(ex.Message);
        }
    }
    public async Task<IServiceResultWithData<User>> MeAsync(int userId, CancellationToken ct = default)
    {
        var user = await _authRepository.GetUserByIdWithRoleAsync(userId, ct);

        if (user is null)
            return new ErrorResultWithData<User>($"Invalid token.");

        return new SuccessResultWithData<User>("Me: ", user);
    }
}