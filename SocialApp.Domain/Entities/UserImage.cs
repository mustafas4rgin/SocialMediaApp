namespace SocialApp.Domain.Entities;

public class UserImage : EntityBase
{
    public int UserId { get; set; }
    public string File { get; set; } = string.Empty;
    //Navigation properties
    public User User { get; set; } = null!;
}