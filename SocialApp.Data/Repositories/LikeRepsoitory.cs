using SocialApp.Data.Contexts;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;

namespace SocialApp.Data.Repositories;

public class LikeRepository : GenericRepository<Like>, ILikeRepository
{
    public LikeRepository(AppDbContext context) : base(context)
    {}
}