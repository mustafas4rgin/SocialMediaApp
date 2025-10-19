namespace SocialApp.Domain.DTOs.Create;

public class CreatePostImageDTO
{
    public string File { get; set; } = string.Empty;
    public int PostId { get; set; }
}