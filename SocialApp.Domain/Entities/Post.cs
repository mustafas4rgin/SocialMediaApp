using SocialApp.Domain.Enums;

namespace SocialApp.Domain.Entities;

public class Post : EntityBase
{
    public string Body { get; set; } = string.Empty;
    public int UserId { get; set; }
    public PostStatus Status { get; set; }
    //Navigation properties
    public User User { get; set; } = null!;
    public ICollection<PostImage>? PostImages { get; set; }
    public ICollection<PostBrutal>? PostBrutals { get; set; }
    public ICollection<Like>? Likes { get; set; }
}