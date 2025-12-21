
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SocialApp.Application.Interfaces;
using SocialApp.Domain.Parameters;

namespace SocialApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : BaseApiController
    {
        private readonly IProfileService _profileService;
        private readonly IMapper _mapper;
        public ProfileController(
            IProfileService profileService,
            IMapper mapper
        )
        {
            _mapper = mapper;
            _profileService = profileService;
        }
        [HttpGet]
        public async Task<IActionResult> GetProfileInfoAsync([FromQuery]QueryParameters param, CancellationToken ct = default)
        {
            var userId = CurrentUserId;

            if (userId is null)
                return Unauthorized("You need to be logged in.");

            var result = await _profileService.GetProfileAsync(userId.Value, param, ct);

            var errorResult = HandleServiceResult(result);

            if (errorResult != null)
                return errorResult;
            
            return Ok(result);
        }
    }
}
