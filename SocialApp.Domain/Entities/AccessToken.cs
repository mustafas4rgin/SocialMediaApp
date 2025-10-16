namespace SocialApp.Domain.Entities;

public sealed class AccessToken : EntityBase
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; } = false;
    public int UserId { get; set; }
    //navigation properties
    public User User { get; set; } = null!;
}