using MediatR;
using SocialApp.Application.Interfaces;
using SocialApp.Application.Services;
using SocialApp.Domain.Entities;
using SocialApp.Domain.Events;

namespace SocialApp.Application.EventHandlers;

public class UpdateProfileEventHandler : INotificationHandler<UpdateProfileEvent>
{
    private readonly INotificationService _notificationService;
    public UpdateProfileEventHandler(
        INotificationService notificationService
    )
    {
        _notificationService = notificationService;
    }
    public async Task Handle(UpdateProfileEvent evt, CancellationToken ct)
    {
        await _notificationService.CreateNotificationAsync(
            new Notification
            {
                Message = "Your profile has been updated successfully.",
                UserId = evt.userId
            }
        );
    }
}