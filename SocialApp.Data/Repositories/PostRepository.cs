using Microsoft.EntityFrameworkCore;
using SocialApp.Data.Contexts;
using SocialApp.Data.Helpers;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.DTOs.List;
using SocialApp.Domain.Entities;

namespace SocialApp.Data.Repositories;

public class PostRepository : GenericRepository<Post>, IPostRepository
{
    private readonly AppDbContext _context;
    public PostRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
    public async Task<List<Post>> GetUsersFeedAsync(int userId, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        var followingIdsQuery = _context.Follows
            .Where(f => f.FollowerId == userId)
            .Select(f => f.FollowingId);

        if (!followingIdsQuery.Any())
            return new();

        return await Query(includeDeleted: false, asNoTracking: true)
                    .Where(p => followingIdsQuery.Contains(p.UserId))
                    .OrderedByNewest()
                    .PagedQuery(pageNumber, pageSize)
                    .Include(p => p.User)
                    .Include(p => p.Likes)
                    .Include(p => p.Comments)
                    .Include(p => p.PostBrutals)
                    .Include(p => p.PostImages)
                    .ToListAsync(ct);
    }
    public async Task<int> CountUsersPostsAsync(int userId, CancellationToken ct = default)
    => await Query(includeDeleted: false, asNoTracking: true)
            .Where(p => p.UserId == userId)
            .CountAsync();

    public async Task<List<PostDTO>> GetUserPostsPagedAsync(int userId, int pageNumber, int pageSize, CancellationToken ct = default)
    => await Query(includeDeleted: false, asNoTracking: true)
                .Where(p => p.UserId == userId)
                .OrderedByNewest()
                .PagedQuery(pageNumber, pageSize)
                .Select(p => new PostDTO
                {
                    Id = p.Id,
                    Body = p.Body,
                    CreatedAt = p.CreatedAt,
                    UserId = p.UserId
                })
                .ToListAsync(ct);
}
