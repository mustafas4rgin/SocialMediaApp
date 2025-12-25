using Microsoft.EntityFrameworkCore;
using SocialApp.Data.Contexts;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.DTOs;
using SocialApp.Domain.DTOs.List;
using SocialApp.Domain.Entities;

namespace SocialApp.Data.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    private readonly AppDbContext _context;
    public UserRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
    public async Task<List<UserRecommendationDto>> GetRecommendedUsersAsync(
    int userId,
    int pageNumber,
    int pageSize,
    CancellationToken ct = default)
    {
        return await _context.Users
            .AsNoTracking()
            .Where(u => u.Id != userId)
            .OrderBy(u => u.Id)
            .Take(5)
            .Select(u => new UserRecommendationDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                IsFollowedByMe = _context.Follows.Any(f =>
                    f.FollowerId == userId &&
                    f.FollowingId == u.Id)
            })
    .ToListAsync(ct);

    }
    public async Task<List<User>> GetAllUsersAsync(CancellationToken ct = default)
    {
        var query = _context.Users;

        return await query.AsNoTracking().ToListAsync(ct);
    }
    public async Task<User?> GetUserByIdAsync(int id, CancellationToken ct = default)
    {
        var query = _context.Users
                        .Where(u => u.Id == id);

        return await query.FirstOrDefaultAsync(ct);
    }
    public async Task<User?> GetProfileAsync(int id, CancellationToken ct = default)
    {
        return await _context.Users
            .AsNoTracking()
            .Where(u => u.Id == id)
            .Include(u => u.Followers)
            .Include(u => u.Followings)
            .Include(u => u.UserImages)
            .FirstOrDefaultAsync(ct);
    }
    public async Task<ProfileHeaderDTO?> GetProfileByUsernameAsync(string userName, CancellationToken ct = default)
    {
        return await _context.Users
            .AsNoTracking()
            .Where(u => u.UserName == userName)
            .Include(u => u.Followers)
            .Include(u => u.Followings)
            .Select(u => new ProfileHeaderDTO
            {
                UserId = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                UserName = u.UserName,

                FollowersCount = u.Followers.Count,
                FollowingsCount = u.Followings.Count,

                FollowersPreview = u.Followers
                    .OrderByDescending(x => x.Id)
                    .Take(10)
                    .Select(f => new UserDTO
                    {
                        Id = f.FollowerId
                    }).ToList(),

                    FollowingsPreview = u.Followings
                    .OrderByDescending(x => x.Id)
                    .Take(10)
                    .Select(f => new UserDTO
                    {
                        Id = f.FollowingId,
                    }).ToList(),
            }).FirstOrDefaultAsync(ct);
    }
    public async Task<ProfileHeaderDTO?> GetProfileHeaderAsync(int userId, CancellationToken ct = default)
    {
        return await _context.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Include(u => u.Followers)
            .Include(u => u.Followings)
            .Select(u => new ProfileHeaderDTO
            {
                UserId = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                UserName = u.UserName,

                FollowersCount = u.Followers.Count,
                FollowingsCount = u.Followings.Count,

                FollowersPreview = u.Followers
                    .OrderByDescending(x => x.Id)
                    .Take(10)
                    .Select(f => new UserDTO
                    {
                        Id = f.FollowerId
                         
                    }).ToList(),

                FollowingsPreview = u.Followings
                    .OrderByDescending(x => x.Id)
                    .Take(10)
                    .Select(f => new UserDTO
                    {
                        Id = f.FollowingId,
                    }).ToList(),
            })
            .FirstOrDefaultAsync(ct);
    }
    public async Task<List<Post>> GetUserPostsAsync(int userId, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        return await _context.Posts
            .AsNoTracking()
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ThenByDescending(p => p.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

}
