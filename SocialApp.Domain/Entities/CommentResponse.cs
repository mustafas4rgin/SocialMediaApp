namespace SocialApp.Domain.Entities;

public class CommentResponse : EntityBase
{
    public int CommentId { get; set; }
    public int UserId { get; set; }
    public string Body { get; set; } = string.Empty;
    //Navigation properties
    public Comment Comment { get; set; } = null!;
    public User User { get; set; } = null!;
}