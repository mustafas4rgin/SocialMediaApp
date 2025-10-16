namespace SocialApp.Domain.Entities;

public sealed class Follow : EntityBase
{
    public int FollowerId { get; set; }
    public int FollowingId { get; set; }
    // Navigation properties
    public User Follower { get; set; } = null!;
    public User Following { get; set; } = null!;
}