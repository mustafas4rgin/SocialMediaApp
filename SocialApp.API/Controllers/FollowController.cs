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
    public class FollowController : GenericController<Follow, FollowDTO, CreateFollowDTO, UpdateFollowDTO>
    {
        private readonly IFollowService _followService;
        private readonly IMapper _mapper;
        public FollowController(
        IValidator<CreateFollowDTO> createValidator,
        IValidator<UpdateFollowDTO> updateValidator,
        IFollowService followService,
        IMapper mapper
        ) : base(createValidator, updateValidator, followService, mapper)
        {
            _followService = followService;
            _mapper = mapper;
        }
        [HttpGet("followers/{followingId}")]
        public  async Task<IActionResult> GetFollowersByFollowingIdAsync([FromRoute]int followingId, [FromQuery]QueryParameters param, CancellationToken ct = default)
        {
            var result = await _followService.GetFollowsByFollowingId(followingId, param, ct);

            var errorResult = HandleServiceResult(result);

            if (errorResult != null)
                return errorResult;

            var followers = result.Data;

            var dto = _mapper.Map<IEnumerable<Follow>>(followers);

            return Ok(
            new
            {
                result.Message,
                Followers = dto
            }
            );
        }
        public override async Task<IActionResult> GetAllAsync([FromQuery] QueryParameters param, CancellationToken ct = default)
        {
            var result = await _followService.GetAllFollowsWithIncludesAsync(param, ct);

            var errorResult = HandleServiceResult(result);

            if (errorResult != null)
                return errorResult;

            var follows = result.Data;

            var dto = _mapper.Map<IEnumerable<FollowDTO>>(follows);

            return Ok(
            new
            {
                result.Message,
                Follows = dto
            }
            );
        }
        public override async Task<IActionResult> GetByIdAsync([FromRoute]int id, [FromQuery]QueryParameters param, CancellationToken ct = default)
        {
            var result = await _followService.GetFollowByIdWithIncludesAsync(id, param, ct);

            var errorResult = HandleServiceResult(result);

            if (errorResult != null)
                return errorResult;

            var follow = result.Data;

            var dto = _mapper.Map<FollowDTO>(follow);

            return Ok(
            new
            {
                result.Message,
                Follow = dto
            }
            );
        }
    }
}
