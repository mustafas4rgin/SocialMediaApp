namespace SocialApp.Domain.Entities;

public class PostImage : EntityBase
{
    public string File { get; set; } = string.Empty;
    public int PostId { get; set; }
    //Navigation properties
    public Post Post { get; set; } = null!;
}