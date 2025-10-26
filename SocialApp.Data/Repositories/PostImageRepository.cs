using SocialApp.Data.Contexts;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;

namespace SocialApp.Data.Repositories;

public class PostImageRepository : GenericRepository<PostImage>, IPostImageRepository
{
    public PostImageRepository(AppDbContext context) : base(context)
    {}
}