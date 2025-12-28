using MediatR;

namespace SocialApp.Domain.Events;

public record CommentEvent(int PostOwnerId, int ActorUserId) : INotification;
