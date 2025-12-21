using SocialApp.Domain.DTOs.List;
using SocialApp.Domain.Entities;

namespace SocialApp.Domain.DTOs;

public class FeedDTO
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public UserDTO User { get; set; } = new UserDTO();
    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
    public List<PostImage>? PostImages { get; set; }
    public List<PostBrutal>? PostBrutals { get; set; }
    public bool IsLikedByMe { get; set; } = false;
}