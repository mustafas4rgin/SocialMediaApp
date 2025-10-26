using FluentValidation;
using Microsoft.Extensions.Logging;
using SocialApp.Application.Interfaces;
using SocialApp.Domain.DTOs;
using SocialApp.Domain.Entities;

namespace SocialApp.Application.Services;

public class PostBrutalService : GenericService<PostBrutal>, IPostBrutalService
{
    public PostBrutalService(
    IValidator<PostBrutal> validator,
    IPostBrutalRepository postBrutalRepository,
    ILogger<PostBrutalService> logger
    ) : base(validator, postBrutalRepository, logger)
    {}
}