namespace SocialApp.Domain.Entities;

public class User : EntityBase
{
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public byte[] PasswordHash { get; set; } = null!;
    public byte[] PasswordSalt { get; set; } = null!;
    //Navigation properties
    public Role Role { get; set; } = null!;
    public ICollection<UserImage> UserImages { get; set; } = null!;
    public ICollection<Post>? Posts { get; set; }
    public ICollection<Follow> Followers { get; set; } = null!;
    public ICollection<Follow> Followings { get; set; } = null!;
}