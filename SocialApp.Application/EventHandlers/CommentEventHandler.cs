using MediatR;
using SocialApp.Application.Interfaces;
using SocialApp.Domain.Entities;
using SocialApp.Domain.Events;

namespace SocialApp.Application.EventHandlers;

public class CommentEventHandler : INotificationHandler<CommentEvent>
{
    private readonly INotificationService _notificationService;
    public CommentEventHandler(
        INotificationService notificationService
    )
    {
        _notificationService = notificationService;
    }
    public async Task Handle(CommentEvent evt, CancellationToken ct)
    {
        if (evt.PostOwnerId == evt.ActorUserId)
            return;

        await _notificationService.CreateNotificationAsync(
            new Notification
            {
                Message = "Yeni yorum geldi.",
                UserId = evt.PostOwnerId
            },
            ct: ct
        );
    }
}