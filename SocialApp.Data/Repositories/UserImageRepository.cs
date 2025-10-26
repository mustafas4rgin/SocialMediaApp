using SocialApp.Data.Contexts;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;

namespace SocialApp.Data.Repositories;

public class UserImageRepository : GenericRepository<UserImage>, IUserImageRepository
{
    public UserImageRepository(AppDbContext context) : base(context) {}
}