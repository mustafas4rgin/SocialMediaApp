namespace SocialApp.Domain.Entities;

public class CreateUserImageDTO
{
    public int UserId { get; set; }
    public string File { get; set; } = string.Empty;
}