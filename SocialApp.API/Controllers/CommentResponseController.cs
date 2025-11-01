using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialApp.API.Controllers;
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
    public class CommentResponseController : GenericController<CommentResponse, CommentResponseDTO, CreateCommentResponseDTO, UpdateCommentResponseDTO>
    {
        private readonly ICommentResponseService _commentResponseSevice;
        private readonly IMapper _mapper;
        public CommentResponseController(
        IValidator<CreateCommentResponseDTO> createValidator,
        IValidator<UpdateCommentResponseDTO> updateValidator,
        IMapper mapper,
        ICommentResponseService commentResponseService
        ) : base(createValidator, updateValidator, commentResponseService, mapper)
        {
            _commentResponseSevice = commentResponseService;
            _mapper = mapper;
        }
        public override async Task<IActionResult> GetAllAsync([FromQuery] QueryParameters param, CancellationToken ct = default)
        {
            var result = await _commentResponseSevice.GetAllCommentResponsesWithIncludesAsync(param, ct);

            var errorResult = HandleServiceResult(result);

            if (errorResult != null)
                return errorResult;

            var responses = result.Data;

            var dto = _mapper.Map<IEnumerable<CommentResponseDTO>>(responses);

            return Ok(
            new
            {
                result.Message,
                Responses = dto
            }
            );
        }
        public override async Task<IActionResult> GetByIdAsync([FromRoute]int id, [FromQuery]QueryParameters param, CancellationToken ct = default)
        {
            var result = await _commentResponseSevice.GetCommentResponseByIdWithIncludesAsync(id, param, ct);

            var errorResult = HandleServiceResult(result);

            if (errorResult != null)
                return errorResult;

            var response = result.Data;

            var dto = _mapper.Map<CommentResponseDTO>(response);

            return Ok(
            new
            {
                result.Message,
                dto
            }
            );
        }
    }
}
