namespace SocialApp.Domain.DTOs.List;

public class NotificationDTO
{
    public int Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsSeen { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
