using Microsoft.EntityFrameworkCore;
using SocialApp.Data.Contexts;
using SocialApp.Data.Helpers;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;

namespace SocialApp.Data.Repositories;

public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
{
    private readonly AppDbContext _context;
    public NotificationRepository(
        AppDbContext context
    ) : base(context)
    {
        _context = context;
    }
    public async Task<List<Notification>> GetNotificationsByUserIdAsync(int userId, CancellationToken ct = default)
    {
        var cutOff = DateTimeOffset.UtcNow.AddDays(-30);
        return await Query(includeDeleted: false, asNoTracking: false)
                        .Where(p => p.UserId == userId && p.CreatedAt >= cutOff)
                        .OrderedByNewest()
                        .ToListAsync(ct);
    }
    
    public async Task<bool> MarkAsSeenAsync(int notificationId, CancellationToken ct = default)
    =>  await _context.Notifications
                .Where(n => n.Id == notificationId && !n.IsDeleted && n.DeletedAt == null && !n.IsSeen)
                .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsSeen, true), ct) > 0;

}
