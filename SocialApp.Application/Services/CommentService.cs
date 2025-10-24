using System.Security.Cryptography.X509Certificates;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog;
using SocialApp.Application.Interfaces;
using SocialApp.Application.Validators;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;
using SocialApp.Domain.Results.Error;

namespace SocialApp.Application.Services;

public class CommentService : GenericService<Comment>, ICommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly IValidator<Comment> _validator;
    private readonly ILogger<CommentService> _logger;
    public CommentService(
    ICommentRepository commentRepository,
    IValidator<Comment> validator,
    ILogger<CommentService> logger
    ) : base(validator, commentRepository, logger)
    {
        _validator = validator;
        _commentRepository = commentRepository;
        _logger = logger;
    }
    //add override
}