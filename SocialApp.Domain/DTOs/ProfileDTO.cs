using SocialApp.Domain.DTOs.List;
using SocialApp.Domain.Entities;

namespace SocialApp.Domain.DTOs;

public class ProfileDTO
{
    public ProfileHeaderDTO HeaderDTO { get; set; } = new();
    public int PostsCount { get; set; }
    public List<PostDTO> Posts { get; set; } = new();
}