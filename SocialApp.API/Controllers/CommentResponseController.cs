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

namespace MyApp.Namespace
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentResponseController : GenericController<CommentResponse, CommentResponseDTO, CreateCommentResponseDTO, UpdateCommentResponseDTO>
    {
        public CommentResponseController(
        IValidator<CreateCommentResponseDTO> createValidator,
        IValidator<UpdateCommentResponseDTO> updateValidator,
        IMapper mapper,
        ICommentResponseService commentResponseService
        ) : base(createValidator, updateValidator, commentResponseService, mapper)
        {}
    }
}
