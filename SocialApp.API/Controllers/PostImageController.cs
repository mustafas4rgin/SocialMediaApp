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
        private readonly IPostImageService _postImageService;
        public PostImageController(
        IValidator<CreatePostImageDTO> createValidator,
        IValidator<UpdatePostImageDTO> updateValidator,
        IPostImageService postImageService,
        IMapper mapper
        ) : base(createValidator, updateValidator, postImageService, mapper)
        {
            _postImageService = postImageService;
        }
        [HttpGet("post-images/{postId}")]
        public async Task<IActionResult> GetPostImagesAsync([FromRoute]int postId, CancellationToken ct = default)
        {
            var result = await _postImageService.GetPostImages(postId);

            var errorResult = HandleServiceResult(result);

            if (errorResult != null)
                return errorResult;
            
            return Ok(result);
        }
    }
}
