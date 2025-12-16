using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using SocialApp.Application.Helpers;
using SocialApp.Application.Interfaces;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;
using SocialApp.Domain.Parameters;
using SocialApp.Domain.Results.Error;
using SocialApp.Domain.Results.Success;

namespace SocialApp.Application.Services;

public class UserService : GenericService<User>, IUserService
{
    private readonly IDistributedCache _cache;
    private readonly IUserRepository _userRepository;
    private readonly ILogger _logger;
    public UserService(
    IDistributedCache cache,
    IValidator<User> validator,
    IUserRepository userRepository,
    ILogger<UserService> logger
    ) : base(validator, userRepository, logger)
    {
        _cache = cache;
        _userRepository = userRepository;
        _logger = logger;
    }
    public async Task<IServiceResultWithData<IEnumerable<User>>> GetAllUsersWithIncludesAsync(QueryParameters param, CancellationToken ct)
    {
        try
        {
            var cacheKey = ListHelper.UsersListKey(param);

            var cached = await CacheHelper.GetTypedAsync<IEnumerable<User>>(_cache, cacheKey, ct);
            if (cached is not null && cached.Any())
                return new SuccessResultWithData<IEnumerable<User>>("Users found (cache)", cached);

            await SemaphoreSlimHelper.LockAsync(cacheKey, ct);
            try
            {
                cached = await CacheHelper.GetTypedAsync<IEnumerable<User>>(_cache, cacheKey, ct);
                if (cached is not null && cached.Any())
                    return new SuccessResultWithData<IEnumerable<User>>("Users found (cache)", cached);

                var users = await _userRepository.GetAllUsersAsync(param.Include, ct);

                await CacheHelper.SetTypedAsync(_cache, cacheKey, users, ct);

                return new SuccessResultWithData<IEnumerable<User>>("Users found.", users);
            }
            finally
            {
                SemaphoreSlimHelper.Release(cacheKey);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while getting users.");
            return new ErrorResultWithData<IEnumerable<User>>("An unexpected error occured.");
        }
    }
    public async Task<IServiceResultWithData<User>> GetUserByIdWithIncludesAsync(int id, QueryParameters param, CancellationToken ct = default)
    {
        try
        {
            var cacheKey = GetById.User(id, param.Include);

            var cached = await CacheHelper.GetTypedAsync<User>(_cache, cacheKey, ct);

            if (cached is not null)
                return new SuccessResultWithData<User>("User found (cached)", cached);

            var user = await _userRepository.GetUserByIdAsync(id, param.Include, ct);

            if (user is null)
                return new ErrorResultWithData<User>($"There is no user with ID : {id}", 404);

            await CacheHelper.SetTypedAsync(_cache, cacheKey, user, ct);

            return new SuccessResultWithData<User>("User found.", user);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("GetUserByIdWithIncludesAsync canceled for id {UserId}", id);
            return new ErrorResultWithData<User>("Operation canceled.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occured while getting user with ID : {id}");
            return new ErrorResultWithData<User>("An unexpected error occured.");
        }
    }
}