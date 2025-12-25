using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using SocialApp.Application.Helpers;
using SocialApp.Application.Interfaces;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.DTOs;
using SocialApp.Domain.DTOs.List;
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

                var posts = await _postRepository.GetAllAsync(
                    param,
                    includeDeleted: false,
                    ct: ct
                );

                if (!posts.Any())
                    return new ErrorResultWithData<IEnumerable<Post>>("There is no post.", 404);

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
            return new ErrorResultWithData<IEnumerable<Post>>("An unexpected error occured.");
        }
    }
    public async Task<IServiceResultWithData<Post>> GetPostByIdWithIncludesAsync(
        int id,
        QueryParameters param,
        CancellationToken ct)
    {
        var cacheKey = GetById.Post(id);

        try
        {
            var cached = await CacheHelper.GetTypedAsync<Post>(_cache, cacheKey, ct);
            if (cached is not null)
            {
                _logger.LogDebug("Post cache hit for id {PostId} key {CacheKey}", id, cacheKey);
                return new SuccessResultWithData<Post>("Post (cache)", cached);
            }

            var post = await _postRepository.GetByIdAsync(id: id, includeDeleted: false, asNoTracking: true);

            if (post is null)
            {
                _logger.LogInformation("Post not found for id {PostId}", id);
                return new ErrorResultWithData<Post>($"There is no post with ID: {id}", 404);
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
    public async Task<IServiceResultWithData<List<FeedDTO>>> GetFeedAsync(
    int userId,
    int pageNumber,
    int pageSize,
    CancellationToken ct = default)
    {
        try
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : pageSize;
            pageSize = pageSize > 50 ? 50 : pageSize;

            var feed = await _postRepository.GetUsersFeedAsync(userId, pageNumber, pageSize, ct);

            var dto = new List<FeedDTO>();

            foreach (var post in feed)
            {
                dto.Add(new FeedDTO
                {
                    Id = post.Id,
                    Content = post.Body,
                    CreatedAt = post.CreatedAt,

                    CommentCount = post.Comments?.Count(c => !c.IsDeleted) ?? 0,
                    LikeCount = post.Likes?.Count(l => !l.IsDeleted) ?? 0,

                    PostImages = post.PostImages?.ToList() ?? new List<PostImage>(),
                    PostBrutals = post.PostBrutals?.ToList() ?? new List<PostBrutal>(),

                    User = new UserDTO
                    {
                        Id = post.UserId,
                        FirstName = post.User is null ? "" : post.User.FirstName,
                        LastName = post.User is null ? "" : post.User.LastName
                    },

                    IsLikedByMe = post.Likes?.Any(l => !l.IsDeleted && l.UserId == userId) ?? false
                });
            }


            if (!dto.Any())
                return new ErrorResultWithData<List<FeedDTO>>("There is no post.", 404);

            return new SuccessResultWithData<List<FeedDTO>>("Feed found.", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting feed.");
            return new ErrorResultWithData<List<FeedDTO>>("An error occured while retrieving feed.");
        }
    }

}
