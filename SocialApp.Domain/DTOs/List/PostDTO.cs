using SocialApp.Domain.Enums;

namespace SocialApp.Domain.DTOs.List;

public class PostDTO
{
    public int Id { get; set; }
    public string Body { get; set; } = string.Empty;
    public int UserId { get; set; }
    public int? MainPostImageId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public PostStatus Status { get; set; }
    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
    public bool IsLikedByMe { get; set; } = false;
}
