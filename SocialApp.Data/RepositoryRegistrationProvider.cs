using Microsoft.Extensions.DependencyInjection;
using SocialApp.Data.Repositories;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.DTOs;

namespace SocialApp.Data;

public class RepositoryRegistrationProvider
{
    public static void RegisterRepositories(IServiceCollection services)
    {
        var servicesToRegister = new (Type Interface, Type Implementation)[]
        {
            (typeof(IGenericRepository<>),typeof(GenericRepository<>)),
            (typeof(IRoleRepository),typeof(RoleRepository)),
            (typeof(IFollowRepository),typeof(FollowRepository)),
            (typeof(ICommentRepository),typeof(CommentRepository)),
            (typeof(ICommentResponseRepository),typeof(CommentResponseRepository)),
            (typeof(ILikeRepository),typeof(LikeRepository)),
            (typeof(IPostRepository), typeof(PostRepository)),
            (typeof(IPostBrutalRepository), typeof(PostBrutalRepository)),
            (typeof(IPostImageRepository),typeof(PostImageRepository)),
            (typeof(IUserRepository),typeof(UserRepository)),
            (typeof(IUserImageRepository),typeof(UserImageRepository))
        };
        foreach (var service in servicesToRegister)
        {
            services.AddTransient(service.Interface, service.Implementation);
        }
    }
}