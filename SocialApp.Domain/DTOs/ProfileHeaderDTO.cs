using SocialApp.Domain.DTOs.List;

namespace SocialApp.Domain.DTOs;

public class ProfileHeaderDTO
{
    public int UserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? UserName { get; set; }

    public int FollowersCount { get; set; }
    public int FollowingsCount { get; set; }

    
    public List<UserDTO> FollowersPreview { get; set; } = new();
    public List<UserDTO> FollowingsPreview { get; set; } = new();
}