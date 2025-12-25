using SocialApp.Domain.Entities;

namespace SocialApp.Domain.Contracts;

public interface INotificationRepository : IGenericRepository<Notification>
{
    Task<List<Notification>> GetNotificationsByUserIdAsync(int userId, CancellationToken ct = default);
    Task<bool> MarkAsSeenAsync(int notificationId, CancellationToken ct = default);
}