using SocialApp.Domain.Entities;

namespace SocialApp.Domain.Contracts;

public interface INotificationRepository
{
    Task<List<Notification>> GetNotificationsByUserIdAsync(int userId, CancellationToken ct = default);
}