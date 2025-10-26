using FluentValidation;
using Microsoft.Extensions.Logging;
using SocialApp.Application.Interfaces;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;

namespace SocialApp.Application.Services;

public class UserService : GenericService<User>, IUserService
{
    public UserService(
    IValidator<User> validator,
    IUserRepository userRepository,
    ILogger<UserService> logger
    ) : base(validator, userRepository, logger)
    {}
}