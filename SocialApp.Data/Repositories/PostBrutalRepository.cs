using Microsoft.EntityFrameworkCore;
using SocialApp.Data.Contexts;
using SocialApp.Data.Helpers;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;

namespace SocialApp.Data.Repositories;

public class PostBrutalRepository : GenericRepository<PostBrutal>, IPostBrutalRepository
{
    private readonly AppDbContext _context;
    public PostBrutalRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
    public async Task<List<PostBrutal>> GetPostBrutalsByPostIdAsync(int postId, CancellationToken ct = default)
    => await Query(includeDeleted: false, asNoTracking: true)
                .Where(pb => pb.PostId == postId)
                .OrderedByNewest()
                .ToListAsync(ct);
}
