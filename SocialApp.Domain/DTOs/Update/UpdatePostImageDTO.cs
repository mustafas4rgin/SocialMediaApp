namespace SocialApp.Domain.DTOs.Update;

public class UpdatePostImageDTO
{
    public string File { get; set; } = string.Empty;
    public int PostId { get; set; }
}