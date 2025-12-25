using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialApp.Application.Interfaces;
using SocialApp.Domain.DTOs.Create;
using SocialApp.Domain.DTOs.List;
using SocialApp.Domain.DTOs.Update;
using SocialApp.Domain.Entities;

namespace SocialApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : GenericController<
    Notification,
    NotificationDTO,
    CreateNotificationDTO,
    UpdateNotificationDTO>
    {
        private readonly INotificationService _notificationService;
        public NotificationController(
            IValidator<CreateNotificationDTO> createValidator,
            IValidator<UpdateNotificationDTO> updateValidator,
            IMapper mapper,
            INotificationService notificationService
        ) : base(createValidator, updateValidator, notificationService, mapper)
        {
            _notificationService = notificationService;
        }
        [HttpGet("notifications")]
        public async Task<IActionResult> GetNotificationsByUserIdAsync(CancellationToken ct = default)
        {
            var userId = CurrentUserId;

            if (userId is null)
                return Unauthorized("You need to login first.");

            var result = await _notificationService.GetNotificationsByUserIdAsync(userId.Value, ct);

            var errorResult = HandleServiceResult(result);

            if (errorResult != null)
                return errorResult;
            
            return Ok(result);
        }
        [HttpPost("notifications/{id}/mark-as-seen")]
        public async Task<IActionResult> MarkAsSeenAsync([FromRoute]int id, CancellationToken ct = default)
        {
            var result = await _notificationService.MarkAsSeenAsync(id, ct);

            var errorResult = HandleServiceResult(result);

            if (errorResult != null)
                return errorResult;

                return Ok(result);
        }
    }
}
