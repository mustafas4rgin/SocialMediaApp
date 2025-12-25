using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialApp.Application.Interfaces;
using SocialApp.Application.Validators.DTO.Update;
using SocialApp.Domain.DTOs.Create;
using SocialApp.Domain.DTOs.List;
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
    }
}
