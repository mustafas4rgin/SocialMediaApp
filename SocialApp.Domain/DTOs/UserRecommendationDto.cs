namespace SocialApp.Domain.DTOs;

public class UserRecommendationDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public bool IsFollowedByMe { get; set; }
}
