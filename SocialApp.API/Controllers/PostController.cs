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
    public class PostController : GenericController<Post, PostDTO, CreatePostDTO, UpdatePostDTO>
    {
        private readonly IValidator<CreatePostDTO> _createValidator;
        private readonly IValidator<UpdatePostDTO> _updateValidator;
        private readonly IMapper _mapper;
        private readonly IPostService _postService;
        public PostController(
        IValidator<CreatePostDTO> createValidator,
        IValidator<UpdatePostDTO> updateValidator,
        IMapper mapper,
        IPostService postService
        ) : base(createValidator, updateValidator, postService, mapper)
        {
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _mapper = mapper;
            _postService = postService;
        }
        [HttpGet("feed")]
        public async Task<IActionResult> GetFeedAsync([FromQuery]QueryParameters param, CancellationToken ct = default)
        {
            var userId = CurrentUserId;

            if (userId is null)
                return Unauthorized("Unauthorized.");

            var result = await _postService.GetFeedAsync(userId.Value, param.PageNumber, param.PageSize, ct);

            var errorResult = HandleServiceResult(result);

            if (errorResult != null)
                return errorResult;
            
            var feed = result.Data;

            return Ok(feed);
        }
    }
}
