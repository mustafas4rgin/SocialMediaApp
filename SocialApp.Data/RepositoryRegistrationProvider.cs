using Microsoft.Extensions.DependencyInjection;
using SocialApp.Data.Repositories;
using SocialApp.Domain.Contracts;

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
            (typeof(ICommentResponseRepository),typeof(CommentResponseRepository))
        };
        foreach (var service in servicesToRegister)
        {
            services.AddTransient(service.Interface, service.Implementation);
        }
    }
}