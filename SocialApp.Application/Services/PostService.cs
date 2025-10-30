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

public class PostService : GenericService<Post>, IPostService
{
    private readonly IDistributedCache _cache;
    private readonly IPostRepository _postRepository;
    private readonly ILogger<PostService> _logger;
    public PostService(
    IDistributedCache cache,
    IPostRepository postRepository,
    IValidator<Post> validator,
    ILogger<PostService> logger
    ) : base(validator, postRepository, logger)
    {
        _cache = cache;
        _postRepository = postRepository;
        _logger = logger;
    }
    public async Task<IServiceResultWithData<IEnumerable<Post>>> GetAllPostsWithIncludesAsync(QueryParameters param, CancellationToken ct)
    {
        try
        {
            var cacheKey = ListHelper.PostsListKey(param);

            var cached = await CacheHelper.GetTypedAsync<IEnumerable<Post>>(_cache, cacheKey, ct);
            if (cached is not null && cached.Any())
                return new SuccessResultWithData<IEnumerable<Post>>("Posts (cache)", cached);

            await SemaphoreSlimHelper.LockAsync(cacheKey, ct);
            try
            {
                cached = await CacheHelper.GetTypedAsync<IEnumerable<Post>>(_cache, cacheKey, ct);
                if (cached is not null && cached.Any())
                    return new SuccessResultWithData<IEnumerable<Post>>("Posts (cache)", cached);

                var query = _postRepository.GetAllActive(ct);

                if (!string.IsNullOrEmpty(param.Include))
                    query = QueryHelper.ApplyIncludesforPost(query, param.Include);

                var posts = await query.ToListAsync(ct);

                if (!posts.Any())
                    return new ErrorResultWithData<IEnumerable<Post>>("There is no post.");

                await CacheHelper.SetTypedAsync(_cache, cacheKey, posts, ct);

                return new SuccessResultWithData<IEnumerable<Post>>("Posts found.", posts);

            }
            finally
            {
                SemaphoreSlimHelper.Release(cacheKey);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while getting posts.");
            return new ErrorResultWithData<IEnumerable<Post>>(ex.Message);
        }
    }
    public async Task<IServiceResultWithData<Post>> GetPostByIdWithIncludesAsync(
        int id,
        QueryParameters param,
        CancellationToken ct)
    {
        var cacheKey = GetById.Post(id, param.Include);

        try
        {
            var cached = await CacheHelper.GetTypedAsync<Post>(_cache, cacheKey, ct);
            if (cached is not null)
            {
                _logger.LogDebug("Post cache hit for id {PostId} key {CacheKey}", id, cacheKey);
                return new SuccessResultWithData<Post>("Post (cache)", cached);
            }

            IQueryable<Post> query = _postRepository.GetAllActive(ct).AsNoTracking();

            if (!string.IsNullOrWhiteSpace(param.Include))
                query = QueryHelper.ApplyIncludesforPost(query, param.Include);

            var post = await query.FirstOrDefaultAsync(p => p.Id == id, ct);

            if (post is null)
            {
                _logger.LogInformation("Post not found for id {PostId}", id);
                return new ErrorResultWithData<Post>($"There is no post with ID: {id}");
            }

            await CacheHelper.SetTypedAsync(_cache, cacheKey, post, ct);

            return new SuccessResultWithData<Post>("Post found.", post);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("GetPostByIdWithIncludesAsync canceled for id {PostId}", id);
            return new ErrorResultWithData<Post>("Operation canceled.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting post with ID {PostId}", id);
            return new ErrorResultWithData<Post>("An error occurred while retrieving the post.");
        }
    }
}