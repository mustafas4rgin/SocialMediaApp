using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialApp.Application.Interfaces;
using SocialApp.Domain.DTOs;
using SocialApp.Domain.DTOs.Create;
using SocialApp.Domain.DTOs.Update;
using SocialApp.Domain.Entities;
using SocialApp.Domain.Parameters;

namespace SocialApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : GenericController<Comment, CommentDTO, CreateCommentDTO, UpdateCommentDTO>
    {
        private readonly ICommentService _commentService;
        private readonly IMapper _mapper;
        public CommentController(
        IValidator<CreateCommentDTO> createValidator,
        IValidator<UpdateCommentDTO> updateValidator,
        ICommentService commentService,
        IMapper mapper
        ) : base(createValidator, updateValidator, commentService, mapper)
        {
            _commentService = commentService;
            _mapper = mapper;
        }
        public override async Task<IActionResult> GetAllAsync([FromQuery] QueryParameters param, CancellationToken ct = default)
        {
            var result = await _commentService.GetAllCommentsWithIncludesAsync(param, ct);

            var errorResult = HandleServiceResult(result);

            if (errorResult != null)
                return errorResult;

            var comments = result.Data;

            var dto = _mapper.Map<IEnumerable<CommentDTO>>(comments);

            return Ok(
                new
                {
                    result.Message,
                    Comments = dto
                }
            );
        }
        public override async Task<IActionResult> GetByIdAsync([FromRoute]int id, [FromQuery]QueryParameters param, CancellationToken ct = default)
        {
            var result = await _commentService.GetCommentByIdWithIncludesAsync(id, param, ct);

            var errorResult = HandleServiceResult(result);

            if (errorResult != null)
                return errorResult;

            var comment = result.Data;

            var dto = _mapper.Map<CommentDTO>(comment);

            return Ok(
            new
            {
                result.Message,
                Comment = dto
            }
            );
        }
    }
}
