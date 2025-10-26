using FluentValidation;
using Microsoft.Extensions.Logging;
using SocialApp.Application.Registrations;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;

namespace SocialApp.Application.Services;

public class LikeService : GenericService<Like>, ILikeService
{
    public LikeService(
    IValidator<Like> validator,
    ILikeRepository likeRepository,
    ILogger<LikeService> logger
    ) : base(validator, likeRepository, logger)
    {}
}