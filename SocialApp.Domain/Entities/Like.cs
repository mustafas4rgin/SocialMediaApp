namespace SocialApp.Domain.Entities;

public sealed class Like : EntityBase
{
    public int PostId { get; set; }
    public int UserId { get; set; }
    //navigation properties
    public Post Post { get; set; } = null!;
    public User User { get; set; } = null!;
}