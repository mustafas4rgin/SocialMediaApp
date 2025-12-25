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
        private readonly IPostBrutalService _postBrutalService;
        public PostBrutalController(
        IValidator<CreatePostBrutalDTO> createValidator,
        IValidator<UpdatePostBrutalDTO> updateValidator,
        IPostBrutalService postBrutalService,
        IMapper mapper
        ) : base(createValidator, updateValidator, postBrutalService, mapper)
        {
            _postBrutalService = postBrutalService;
        }
        [HttpGet("post-brutals/{postId}")]
        public async Task<IActionResult> GetPostBrutalsByPostIdAsync([FromRoute]int postId, CancellationToken ct = default)
        {
            var result = await _postBrutalService.GetPostBrutalByPostIdAsync(postId, ct);

            var errorResult = HandleServiceResult(result);

            if (errorResult != null)
                return errorResult;

            return Ok(result);
        }
    }
}
