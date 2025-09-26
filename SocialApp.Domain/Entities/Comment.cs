namespace SocialApp.Domain.Entities;

public class Comment : EntityBase
{
    public int PostId { get; set; }
    public int UserId { get; set; }
    public string Body { get; set; } = string.Empty;
    //Navigation properties
    public Post Post { get; set; } = null!;
    public User User { get; set; } = null!;
}