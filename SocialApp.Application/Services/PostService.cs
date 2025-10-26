using FluentValidation;
using Microsoft.Extensions.Logging;
using SocialApp.Application.Interfaces;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;

namespace SocialApp.Application.Services;

public class PostService : GenericService<Post>, IPostService
{
    public PostService(
    IPostRepository postRepository,
    IValidator<Post> validator,
    ILogger<PostService> logger
    ) : base(validator, postRepository, logger)
    {}
}