using FluentValidation;
using Microsoft.Extensions.Logging;
using SocialApp.Application.Interfaces;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;

namespace SocialApp.Application.Services.CommentResponseService;

public class CommentResponseService : GenericService<CommentResponse>, ICommentResponseService
{
    public CommentResponseService(
    IValidator<CommentResponse> validator,
    ICommentResponseRepository commentResponseRepository,
    ILogger<CommentResponseService> logger
    ) : base(validator, commentResponseRepository, logger)
    {}
}