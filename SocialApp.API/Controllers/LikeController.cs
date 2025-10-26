using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialApp.Application.Registrations;
using SocialApp.Domain.DTOs.Create;
using SocialApp.Domain.DTOs.List;
using SocialApp.Domain.DTOs.Update;
using SocialApp.Domain.Entities;

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
    }
}
