using SocialApp.Domain.Contracts;
using SocialApp.Domain.DTOs.List;
using SocialApp.Domain.Entities;

namespace SocialApp.Application.Interfaces;

public interface INotificationService
{
    Task<IServiceResult> MarkAsSeenAsync(int notificationId, CancellationToken ct = default);
    Task<IServiceResult> CreateNotificationAsync(Notification notification, CancellationToken ct = default);
    Task<IServiceResultWithData<List<NotificationDTO>>> GetNotificationsByUserIdAsync(int userId, CancellationToken ct = default);
}