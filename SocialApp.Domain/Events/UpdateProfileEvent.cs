using MediatR;

namespace SocialApp.Domain.Events;

public record UpdateProfileEvent(
    int userId
) : INotification;