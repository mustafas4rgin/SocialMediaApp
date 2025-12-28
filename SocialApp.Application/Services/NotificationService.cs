using FluentValidation;
using Microsoft.Extensions.Logging;
using SocialApp.Application.Interfaces;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.DTOs.Create;
using SocialApp.Domain.DTOs.List;
using SocialApp.Domain.Entities;
using SocialApp.Domain.Results.Error;
using SocialApp.Domain.Results.Success;

namespace SocialApp.Application.Services;

public class NotificationService : INotificationService
{
    private readonly IValidator<Notification> _validator;
    private readonly ILogger<NotificationService> _logger;
    private readonly INotificationRepository _notificationRepository;
    private readonly IUserRepository _userRepository;
    public NotificationService(
        INotificationRepository notificationRepository,
        ILogger<NotificationService> logger,
        IValidator<Notification> validator,
        IUserRepository userRepository
    )
    {
        _validator = validator;
        _logger = logger;
        _notificationRepository = notificationRepository;
        _userRepository = userRepository;
    }
    public async Task<IServiceResult> CreateNotificationAsync(
        Notification notification,
        CancellationToken ct = default
    )
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(notification, ct);

            if (!validationResult.IsValid)
                return new ErrorResult(string.Join(" | ",
                    validationResult.Errors.Select(e => e.ErrorMessage)));
            
            await _notificationRepository.AddAsync(notification, ct);
            await _notificationRepository.SaveChangesAsync(ct);

            return new SuccessResult("Notification created successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred.");
            return new ErrorResult("An error occurred while creating notification.");
        }
    }
    public async Task<IServiceResult> MarkAsSeenAsync(int notificationId, CancellationToken ct = default)
    {
        try
        {
            var updated = await _notificationRepository.MarkAsSeenAsync(notificationId, ct);
            if (!updated)
                return new ErrorResult($"There is no notification with that ID : {notificationId}.", 404);

            return new SuccessResult("Notification successfully marked as seen.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occured.");
            return new ErrorResult("An error occured while marking notification as seen.");
        }
    }
    public async Task<IServiceResultWithData<List<NotificationDTO>>> GetNotificationsByUserIdAsync(int userId, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByIdAsync(
            id: userId,
            includeDeleted: false,
            asNoTracking: false,
            ct: ct
        );

        if (user is null)
            return new ErrorResultWithData<List<NotificationDTO>>($"There is no user with ID : {userId}.");

        try
        {
            var notifications = await _notificationRepository.GetNotificationsByUserIdAsync(
                userId: userId,
                ct: ct
            );

            if (!notifications.Any())
                return new SuccessResultWithData<List<NotificationDTO>>("There is no notification right now.", new());

            return new SuccessResultWithData<List<NotificationDTO>>("Notifications found.",
             notifications
             .Select(n => new NotificationDTO
             {
                 Id = n.Id,
                 Message = n.Message,
                 IsSeen = n.IsSeen,
                 CreatedAt = n.CreatedAt
             })
             .ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Un unknown error occurred.");
            return new ErrorResultWithData<List<NotificationDTO>>($"An unknown error occured while getting notifications of user with ID : {userId}.");
        }
    }
}
