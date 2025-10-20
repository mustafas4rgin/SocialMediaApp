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

namespace SocialApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : GenericController<Role, RoleDTO, CreateRoleDTO, UpdateRoleDTO>
    {
        private readonly IValidator<CreateRoleDTO> _createRoleValidator;
        private readonly IValidator<UpdateRoleDTO> _updateRoleValidator;
        private readonly IMapper _mapper;
        private readonly IGenericService<Role> _genericService;
        public RoleController(
        IValidator<CreateRoleDTO> createRoleValidator,
        IValidator<UpdateRoleDTO> updateRoleValidator,
        IMapper mapper,
        IGenericService<Role> genericService
        ) : base(createRoleValidator, updateRoleValidator, genericService, mapper)
        {
            _createRoleValidator = createRoleValidator;
            _updateRoleValidator = updateRoleValidator;
            _mapper = mapper;
            _genericService = genericService;
        }
    }
}
