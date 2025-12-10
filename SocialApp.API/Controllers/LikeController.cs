using AutoMapper;
using FluentValidation;
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
    public class LikeController : GenericController<Like, LikeDTO, CreateLikeDTO, UpdateLikeDTO>
    {
        private readonly IValidator<CreateLikeDTO> _createValidator;
        private readonly IValidator<UpdateLikeDTO> _updateValidator;
        private readonly IMapper _mapper;
        private readonly ILikeService _likeService;
        public LikeController(
        IValidator<CreateLikeDTO> createvalidator,
        IValidator<UpdateLikeDTO> updateValidator,
        IMapper mapper,
        ILikeService likeService
        ) : base(createvalidator, updateValidator, likeService, mapper)
        {
            _createValidator = createvalidator;
            _updateValidator = updateValidator;
            _mapper = mapper;
            _likeService = likeService;
        }
        public override async Task<IActionResult> GetByIdAsync([FromRoute]int id, [FromQuery]QueryParameters param, CancellationToken ct = default)
        {
            var result = await _likeService.GetLikeByIdWithIncludesAsync(id, param, ct);

            var errorResult = HandleServiceResult(result);

            if (errorResult != null)
                return errorResult;

            var like = result.Data;

            var dto = _mapper.Map<LikeDTO>(like);

            return Ok(
            new
            {
                result.Message,
                Like = dto
            }
            );
        }
        public override async Task<IActionResult> GetAllAsync([FromQuery]QueryParameters param, CancellationToken ct = default)
        {
            var result = await _likeService.GetAllLikesWithIncludesAsync(param, ct);

            var errorResult = HandleServiceResult(result);

            if (errorResult != null)
                return errorResult;

            var likes = result.Data;

            var dto = _mapper.Map<IEnumerable<Like>>(likes);

            return Ok(
            new
            {
                result.Message,
                Likes = dto
            }
            );
        }
        public override async Task<IActionResult> AddAsync([FromBody]CreateLikeDTO dto, CancellationToken ct = default)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
                return HandleValidationErrors(validationResult.Errors);

            var addingLike = _mapper.Map<Like>(dto);

            var addingResult = await _likeService.AddAsync(addingLike, ct);

            var errorResult = HandleServiceResult(addingResult);

            if (errorResult != null)
                return errorResult;

            return Ok(
            new
            {
                addingResult.Message,
                Like = addingLike
            }
            );
        }
    }
}
