using Microsoft.Extensions.DependencyInjection;
using SocialApp.Application.Interfaces;
using SocialApp.Application.Services;

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
            (typeof(ICommentService),typeof(CommentService))
        };

        foreach (var service in servicesToRegister)
        {
            services.AddTransient(service.Interface, service.Implementation);
        }
    }
}