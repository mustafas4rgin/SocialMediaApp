using SocialApp.Data.Contexts;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;

namespace SocialApp.Data.Repositories;

public class PostBrutalRepository : GenericRepository<PostBrutal>, IPostBrutalRepository
{
    public PostBrutalRepository(AppDbContext context) : base(context)
    {}
}