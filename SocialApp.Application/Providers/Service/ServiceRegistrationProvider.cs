using Microsoft.Extensions.DependencyInjection;
using SocialApp.Application.Interfaces;
using SocialApp.Application.Registrations;
using SocialApp.Application.Services;
using SocialApp.Application.Services.CommentResponseService;

namespace SocialApp.Application.Providers.Service;

public class ServiceRegistrationProvider
{
    public static void RegisterServices(IServiceCollection services)
    {
        var servicesToRegister = new (Type Interface, Type Implementation)[]
        {
            (typeof(IGenericService<>),typeof(GenericService<>)),
            (typeof(IRoleService),typeof(RoleService)),
            (typeof(IFollowService),typeof(FollowService)),
            (typeof(ICommentResponseService), typeof(CommentResponseService)),
            (typeof(ILikeService), typeof(LikeService)),
            (typeof(IPostBrutalService), typeof(PostBrutalService)),
            (typeof(IPostImageService), typeof(PostImageService)),
            (typeof(IUserService), typeof(UserService))
        };

        foreach (var service in servicesToRegister)
        {
            services.AddTransient(service.Interface, service.Implementation);
        }
    }
}