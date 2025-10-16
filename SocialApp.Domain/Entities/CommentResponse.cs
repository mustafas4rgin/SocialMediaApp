namespace SocialApp.Domain.Entities;

public sealed class CommentResponse : Comment
{
    public int CommentId { get; set; }
    //Navigation properties
    public Comment Comment { get; set; } = null!;
}