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
    public class FollowController : GenericController<Follow, FollowDTO, CreateFollowDTO, UpdateFollowDTO>
    {
        public FollowController(
        IValidator<CreateFollowDTO> createValidator,
        IValidator<UpdateFollowDTO> updateValidator,
        IFollowService followService,
        IMapper mapper
        ) : base(createValidator, updateValidator, followService, mapper)
        {}
    }
}
