
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SocialApp.Application.Interfaces;
using SocialApp.Domain.DTOs;
using SocialApp.Domain.Parameters;

namespace SocialApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : BaseApiController
    {
        private readonly IProfileService _profileService;
        private readonly IMapper _mapper;
        private readonly IValidator<UpdateProfileDTO> _validator;
        public ProfileController(
            IProfileService profileService,
            IMapper mapper,
            IValidator<UpdateProfileDTO> validator
        )
        {
            _validator = validator;
            _mapper = mapper;
            _profileService = profileService;
        }
        [HttpPost("update")]
        public async Task<IActionResult> UpdateProfileAsync([FromBody]UpdateProfileDTO dTO, CancellationToken ct = default)
        {
            var userId = CurrentUserId;

            if (userId is null)
                return Unauthorized("");
            
            var validationResult = await _validator.ValidateAsync(dTO, ct);

            if (!validationResult.IsValid)
                return HandleValidationErrors(validationResult.Errors);

            var result = await _profileService.UpdateProfileAsync(userId.Value, dTO, ct);

            var errorResult = HandleServiceResult(result);

            if (errorResult != null)
                return errorResult;
            
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetProfileInfoAsync([FromQuery]QueryParameters param, CancellationToken ct = default)
        {
            var userId = CurrentUserId;

            if (userId is null)
                return Unauthorized("You need to be logged in.");

            var result = await _profileService.GetProfileAsync(userId.Value, param, userId.Value, ct);

            var errorResult = HandleServiceResult(result);

            if (errorResult != null)
                return errorResult;
            
            return Ok(result);
        }
        [HttpGet("{userName}")]
        public async Task<IActionResult> GetProfileInfoWithUsernameAsync([FromQuery]QueryParameters param, string userName, CancellationToken ct = default)
        {
            var result = await _profileService.GetProfileWithUsernameAsync(userName, param, CurrentUserId, ct);

            var errorResult = HandleServiceResult(result);

            if (errorResult != null)
                return errorResult;
            
            return Ok(result);
        }
    }
}
