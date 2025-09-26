namespace SocialApp.Domain.Entities;

public class Follow : EntityBase
{
    public int FollowedId { get; set; }
    public int FollowingId { get; set; }
    // Navigation properties
    public User Follower { get; set; } = null!;
    public User Following { get; set; } = null!;
}