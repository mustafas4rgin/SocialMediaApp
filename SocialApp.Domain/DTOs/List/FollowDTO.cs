namespace SocialApp.Domain.DTOs.List;

public class FollowDTO
{
    public int Id { get; set; }
    public int FollowerId { get; set; }
    public int FollowingId { get; set; }
}