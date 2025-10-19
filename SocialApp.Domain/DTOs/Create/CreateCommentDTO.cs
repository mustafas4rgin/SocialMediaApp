namespace SocialApp.Domain.DTOs.Create;

public class CreateCommentDTO
{
    public int PostId { get; set; }
    public int UserId { get; set; }
    public string Body { get; set; } = string.Empty;
}