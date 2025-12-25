using Microsoft.EntityFrameworkCore;
using SocialApp.Data.Contexts;
using SocialApp.Data.Helpers;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;

namespace SocialApp.Data.Repositories;

public class PostImageRepository : GenericRepository<PostImage>, IPostImageRepository
{
    private readonly AppDbContext _context;
    public PostImageRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
    public async Task<List<PostImage>> GetPostImages(int postId, CancellationToken ct = default)
    => await Query(includeDeleted: false, asNoTracking: true)
                .Where(pi => pi.PostId == postId)
                .OrderedByNewest()
                .ToListAsync(ct);
}
