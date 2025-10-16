namespace SocialApp.Domain.Entities;

public sealed class RefreshToken : EntityBase
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt {get; set;}
    public bool IsUsed { get; set; } = false;
    public bool IsRevoked { get; set; } = false;
    public int UserId { get; set; }
    //navigation properties
    public User User { get; set; } = null!;
}