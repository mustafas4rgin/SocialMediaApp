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
    public class NotificationController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IValidator<CreateNotificationDTO> _createValidator;
        private readonly INotificationService _notificationService;
        public NotificationController(
            IValidator<CreateNotificationDTO> createValidator,
            INotificationService notificationService,
            IMapper mapper
        )
        {
            _createValidator = createValidator;
            _notificationService = notificationService;
            _mapper = mapper;
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateNotificationAsync([FromBody]CreateNotificationDTO dto, CancellationToken ct = default)
        {
            var validationResult = await _createValidator.ValidateAsync(dto, ct);

            if (!validationResult.IsValid)
                return HandleValidationErrors(validationResult.Errors);

            var notification = _mapper.Map<Notification>(dto);

            var result = await _notificationService.CreateNotificationAsync(notification, ct);

            var errorResult = HandleServiceResult(result);

            if (errorResult != null)
                return errorResult;

            return Ok(result);
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
