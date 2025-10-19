namespace SocialApp.Domain.DTOs.Update;

public class UpdateCommentDTO
{
    public int PostId { get; set; }
    public int UserId { get; set; }
    public string Body { get; set; } = string.Empty;
}