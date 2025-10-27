using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SocialApp.Domain.Entities;
using SocialApp.Domain.Parameters;

namespace SocialApp.Application.Helpers;

public static class TokenHelper
{
    public static JwtSecurityToken GenerateAccessToken(User user, JwtOptions jwt)
    {
        var claims = new List<Claim>
{
new(ClaimTypes.NameIdentifier, user.Id.ToString()),
new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
new(ClaimTypes.Role, user.Role?.Name ?? "User"),
new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
};

        var keyBytes = Encoding.UTF8.GetBytes(jwt.Key);
        if (keyBytes.Length < 32) // 256-bit
            throw new InvalidOperationException("Jwt:Key must be at least 256 bits (32+ bytes).");

        var key = new SymmetricSecurityKey(keyBytes);
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
        issuer: jwt.Issuer,
        audience: jwt.Audience,
        claims: claims,
        notBefore: DateTime.UtcNow,
        expires: DateTime.UtcNow.AddMinutes(jwt.AccessTokenMinutes),
        signingCredentials: creds
        );

        return token;
    }

    public static RefreshToken GenerateRefreshToken(User user, JwtOptions jwt)
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        var token = Base64UrlEncoder.Encode(bytes);

        var refreshToken = new RefreshToken
        {
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddDays(jwt.RefreshTokenDays),
            UserId = user.Id
        };

        return refreshToken;
    }
}