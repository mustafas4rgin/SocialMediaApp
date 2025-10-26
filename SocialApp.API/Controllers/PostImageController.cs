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
    public class PostImageController : GenericController<PostImage, PostImageDTO, CreatePostImageDTO, UpdatePostImageDTO>
    {
        public PostImageController(
        IValidator<CreatePostImageDTO> createValidator,
        IValidator<UpdatePostImageDTO> updateValidator,
        IPostImageService postImageService,
        IMapper mapper
        ) : base(createValidator, updateValidator, postImageService, mapper)
        {}
    }
}
