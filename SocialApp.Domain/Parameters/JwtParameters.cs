namespace SocialApp.Domain.Parameters;

public class JwtParameters
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
}