using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialApp.Application.Interfaces;
using SocialApp.Domain.DTOs.List;
using SocialApp.Domain.DTOs.Update;
using SocialApp.Domain.Entities;

namespace SocialApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserImageController : GenericController<UserImage, UserImageDTO, CreateUserImageDTO, UpdateUserImageDTO>
    {
        public UserImageController(
        IValidator<CreateUserImageDTO> createValidator,
        IValidator<UpdateUserImageDTO> updateValidator,
        IUserImageService userImageService,
        IMapper mapper
        ) : base(createValidator, updateValidator, userImageService, mapper)
        {}
    }
}
