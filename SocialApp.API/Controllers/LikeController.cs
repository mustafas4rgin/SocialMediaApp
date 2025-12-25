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
