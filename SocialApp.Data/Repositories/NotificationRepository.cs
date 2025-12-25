using Microsoft.EntityFrameworkCore;
using SocialApp.Data.Contexts;
using SocialApp.Data.Helpers;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;

namespace SocialApp.Data.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly AppDbContext _context;
    public NotificationRepository(
        AppDbContext context
    )
    {
        _context = context;
    }
    public async Task<List<Notification>> GetNotificationsByUserIdAsync(int userId, CancellationToken ct = default)
    => await _context.Notifications
                    .Where(p => p.UserId == userId)
                    .OrderedByNewest()
                    .ToListAsync(ct);
}
