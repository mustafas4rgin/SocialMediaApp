namespace SocialApp.API.DTOs.List;

public class RoleDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
}