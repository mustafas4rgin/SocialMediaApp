using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialApp.Application.Interfaces;
using SocialApp.Domain.DTOs.Create;
using SocialApp.Domain.DTOs.List;
using SocialApp.Domain.DTOs.Update;
using SocialApp.Domain.Entities;
using SocialApp.Domain.Parameters;

namespace SocialApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : GenericController<User, UserDTO, CreateUserDTO, UpdateUserDTO>
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public UserController(
        IValidator<CreateUserDTO> createValidator,
        IValidator<UpdateUserDTO> updateValidator,
        IUserService userService,
        IMapper mapper
        ) : base(createValidator, updateValidator, userService, mapper)
        {
            _mapper = mapper;
            _userService = userService;
        }
        public override async Task<IActionResult> GetAllAsync([FromQuery] QueryParameters param, CancellationToken ct = default)
        {
            var result = await _userService.GetAllUsersWithIncludesAsync(param, ct);

            var errorResult = HandleServiceResult(result);

            if (errorResult != null)
                return errorResult;

            var users = result.Data;

            var dto = _mapper.Map<IEnumerable<UserDTO>>(users);

            return Ok(
            new
            {
                result.Message,
                User = dto
            }
            );
        }
        public override async Task<IActionResult> GetByIdAsync([FromRoute]int id, [FromQuery]QueryParameters param, CancellationToken ct = default)
        {
            var result = await _userService.GetUserByIdWithIncludesAsync(id, param, ct);

            var errorResult = HandleServiceResult(result);

            if (errorResult != null)
                return errorResult;

            var user = result.Data;

            var dto = _mapper.Map<UserDTO>(user);

            return Ok(
            new
            {
                result.Message,
                User = dto
            }
            );
        }
    }
}
