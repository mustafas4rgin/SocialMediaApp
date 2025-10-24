namespace SocialApp.Domain.DTOs;

public class CommentDTO
{
    public int PostId { get; set; }
    public int UserId { get; set; }
    public string Body { get; set; } = string.Empty;
}