namespace SocialApp.Domain.Entities;

public class Like : EntityBase
{
    public int PostId { get; set; }
    public int UserId { get; set; }
}