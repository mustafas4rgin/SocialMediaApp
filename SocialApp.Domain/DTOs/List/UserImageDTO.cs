namespace SocialApp.Domain.DTOs.List;

public class UserImageDTO
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string File { get; set; } = string.Empty;
}