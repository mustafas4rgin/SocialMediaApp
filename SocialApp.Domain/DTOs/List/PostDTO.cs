using SocialApp.Domain.Enums;

namespace SocialApp.Domain.DTOs.List;

public class PostDTO
{
    public string Body { get; set; } = string.Empty;
    public int UserId { get; set; }
    public PostStatus Status { get; set; }
}