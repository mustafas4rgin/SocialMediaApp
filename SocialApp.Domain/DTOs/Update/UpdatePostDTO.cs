using SocialApp.Domain.Enums;

namespace SocialApp.Domain.DTOs.Update;

public class UpdatePostDTO
{
    public string Body { get; set; } = string.Empty;
    public int UserId { get; set; }
    public PostStatus Status { get; set; }
}