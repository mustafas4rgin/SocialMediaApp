namespace SocialApp.Domain.Entities;

public class Notification : EntityBase
{
    public string Message { get; set; } = string.Empty;
    public int UserId { get; set; }
    public bool IsSeen { get; set; } = false;
    //navigation properties
    public User User { get; set; } = new();
}