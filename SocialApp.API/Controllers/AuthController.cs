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
        public async Task<IActionResult> LoginAsync([FromBody]LoginDTO dto, CancellationToken ct)
        {
            var loginResult = await _authService.LoginAsync(dto, ct);

            var errorResult = HandleServiceResult(loginResult);

            if (errorResult != null)
                return errorResult;

            var token = loginResult.Data;

            return Ok(token);
        }
    }
}
