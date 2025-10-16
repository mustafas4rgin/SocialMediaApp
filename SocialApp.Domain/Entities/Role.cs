namespace SocialApp.Domain.Entities;

public sealed class Role : EntityBase
{
    public string Name { get; set; } = string.Empty;
    //Navigation properties
    public ICollection<User> Users { get; set; } = null!;
}