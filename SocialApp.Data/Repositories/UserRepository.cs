using Microsoft.EntityFrameworkCore;
using SocialApp.Data.Contexts;
using SocialApp.Data.Helpers;
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
    => await Query(includeDeleted: false, asNoTracking: true)
                .Where(u => u.Id != userId)
                .OrderedByNewest()
                .PagedQuery(pageNumber, 5)
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

    public async Task<User?> GetProfileAsync(int id, CancellationToken ct = default)
    => await Query(includeDeleted: false, asNoTracking: true)
                .Where(u => u.Id == id)
                .Include(u => u.Followers)
                .Include(u => u.Followings)
                .Include(u => u.UserImages)
                .FirstOrDefaultAsync(ct);

    public async Task<ProfileHeaderDTO?> GetProfileByUsernameAsync(string userName, CancellationToken ct = default)
    => await Query(includeDeleted: false, asNoTracking: true)
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
    public async Task<ProfileHeaderDTO?> GetProfileHeaderAsync(int userId, CancellationToken ct = default)
    => await Query(includeDeleted: false, asNoTracking: true)
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
