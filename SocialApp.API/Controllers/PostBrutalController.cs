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
    public class PostBrutalController : GenericController<PostBrutal, PostBrutalDTO, CreatePostBrutalDTO, UpdatePostBrutalDTO>
    {
        public PostBrutalController(
        IValidator<CreatePostBrutalDTO> createValidator,
        IValidator<UpdatePostBrutalDTO> updateValidator,
        IPostBrutalService postBrutalService,
        IMapper mapper
        ) : base(createValidator, updateValidator, postBrutalService, mapper)
        {}
    }
}
