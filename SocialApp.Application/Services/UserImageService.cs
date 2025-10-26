using FluentValidation;
using Microsoft.Extensions.Logging;
using SocialApp.Application.Interfaces;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;

namespace SocialApp.Application.Services;

public class UserImageService : GenericService<UserImage>, IUserImageService
{
    public UserImageService(
    IValidator<UserImage> validator,
    IUserImageRepository userImageRepository,
    ILogger<UserImageService> logger
    ) : base(validator, userImageRepository, logger) {}
}