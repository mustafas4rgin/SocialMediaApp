namespace SocialApp.Domain.DTOs.Update;

public class UpdateNotificationDTO
{
    public string Message { get; set; } = string.Empty;
    public int UserId { get; set; }
}