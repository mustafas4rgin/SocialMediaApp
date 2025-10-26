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
    public class UserController : GenericController<User, UserDTO, CreateUserDTO, UpdateUserDTO>
    {
        public UserController(
        IValidator<CreateUserDTO> createValidator,
        IValidator<UpdateUserDTO> updateValidator,
        IUserService userService,
        IMapper mapper
        ) : base(createValidator, updateValidator, userService, mapper)
        {}
    }
}
