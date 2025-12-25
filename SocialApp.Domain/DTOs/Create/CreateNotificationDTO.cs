namespace SocialApp.Domain.DTOs.Create;

public class CreateNotificationDTO
{
    public string Message { get; set; } = string.Empty;
    public int UserId { get; set; }
}