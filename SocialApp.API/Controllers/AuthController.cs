using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialApp.API.Controllers;
using SocialApp.Application.Interfaces;
using SocialApp.Application.Services;
using SocialApp.Domain.DTOs.Auth;

namespace SocialApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseApiController
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDTO dto, CancellationToken ct)
        {
            var loginResult = await _authService.LoginAsync(dto, ct);

            var errorResult = HandleServiceResult(loginResult);

            if (errorResult != null)
                return errorResult;

            var token = loginResult.Data;

            return Ok(token);
        }
        [HttpGet("me")]
        public async Task<IActionResult> MeAsync(CancellationToken ct = default)
        {
            var userId = CurrentUserId;

            if (userId == null)
                return Unauthorized("Invalid token.");

            var result = await _authService.MeAsync(userId.Value, ct);

            if (!result.Success)
                return BadRequest(result.Message);

            var user = result.Data;

            return Ok(new
            {
                FirstName = user.FirstName + " " + user.LastName,
                Role = user.Role.Name,
            });
        }
    }
}
