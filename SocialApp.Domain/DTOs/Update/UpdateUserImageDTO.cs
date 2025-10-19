namespace SocialApp.Domain.DTOs.Update;

public class UpdateUserImageDTO
{
    public int UserId { get; set; }
    public string File { get; set; } = string.Empty;
}