using FluentValidation;
using Microsoft.Extensions.Logging;
using SocialApp.Application.Interfaces;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;

namespace SocialApp.Application.Services;

public class PostImageService : GenericService<PostImage>, IPostImageService
{
    public PostImageService(
    IValidator<PostImage> validator,
    IPostImageRepository postImageRepository,
    ILogger<PostImageService> logger
    ) : base(validator, postImageRepository, logger)
    {}
}