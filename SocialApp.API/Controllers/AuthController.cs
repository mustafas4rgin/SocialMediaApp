using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> MeAsync(CancellationToken ct = default)
        {
            var userId = CurrentUserId;
            if (userId is null)
                return Unauthorized("Invalid token.");

            var authHeader = Request.Headers["Authorization"].ToString();
            var token = authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                ? authHeader.Substring("Bearer ".Length).Trim()
                : null;

            if (string.IsNullOrWhiteSpace(token))
                return Unauthorized("Token missing.");

            var result = await _authService.MeAsync(userId.Value, token, ct);

            if (!result.Success)
                return Unauthorized(result.Message);

            var user = result.Data;
            return Ok(new
            {
                FirstName = $"{user.FirstName} {user.LastName}",
                Role = user.Role.Name
            });
        }
        [HttpGet("logout")]
        public async Task<IActionResult> LogOutAsync(CancellationToken ct = default)
        {
            var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
            var token = authHeader.StartsWith("Bearer ") ? authHeader.Substring("Bearer ".Length) : authHeader;

            var result = await _authService.LogOutAsync(token);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
