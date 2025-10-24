using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialApp.Application.Interfaces;
using SocialApp.Domain.DTOs;
using SocialApp.Domain.DTOs.Create;
using SocialApp.Domain.DTOs.Update;
using SocialApp.Domain.Entities;

namespace SocialApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : GenericController<Comment, CommentDTO, CreateCommentDTO, UpdateCommentDTO>
    {
        public CommentController(
        IValidator<CreateCommentDTO> createValidator,
        IValidator<UpdateCommentDTO> updateValidator,
        ICommentService commentService,
        IMapper mapper
        ) : base(createValidator, updateValidator, commentService, mapper)
        {}
    }
}
