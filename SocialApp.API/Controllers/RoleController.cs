using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialApp.API.DTOs.List;
using SocialApp.Application.Interfaces;
using SocialApp.Application.Validators.DTO.Update;
using SocialApp.Domain.DTOs.Create;
using SocialApp.Domain.DTOs.Update;
using SocialApp.Domain.Entities;
using SocialApp.Domain.Parameters;

namespace SocialApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : GenericController<Role, RoleDTO, CreateRoleDTO, UpdateRoleDTO>
    {
        private readonly IValidator<CreateRoleDTO> _createRoleValidator;
        private readonly IValidator<UpdateRoleDTO> _updateRoleValidator;
        private readonly IMapper _mapper;
        private readonly IRoleService _roleService;
        public RoleController(
        IValidator<CreateRoleDTO> createRoleValidator,
        IValidator<UpdateRoleDTO> updateRoleValidator,
        IMapper mapper,
        IRoleService roleService
        ) : base(createRoleValidator, updateRoleValidator, roleService, mapper)
        {
            _createRoleValidator = createRoleValidator;
            _updateRoleValidator = updateRoleValidator;
            _mapper = mapper;
            _roleService = roleService;
        }
        [HttpGet("with-includes")]
        public async Task<IActionResult> GetAllWithIncludesAsync([FromQuery] QueryParameters param, CancellationToken ct)
        {
            var result = await _roleService.GetRolesWithIncludesAsync(param, ct);

            var errorResult = HandleServiceResult(result);

            if (errorResult != null)
                return errorResult;

            var roles = result.Data;

            var dto = _mapper.Map<IEnumerable<RoleDTO>>(roles);

            return Ok(dto);
        }
        [HttpGet("{id:int}/with-includes")]
        public async Task<IActionResult> GetByIdWithIncludesAsync([FromRoute]int id, [FromQuery]QueryParameters param, CancellationToken ct)
        {
            var result = await _roleService.GetRoleByIdWithIncludesAsync(id, param, ct);

            var errorResult = HandleServiceResult(result);

            if (errorResult != null)
                return errorResult;

            var role = result.Data;

            var dto = _mapper.Map<RoleDTO>(role);

            return Ok(dto);
        }
    }
}
