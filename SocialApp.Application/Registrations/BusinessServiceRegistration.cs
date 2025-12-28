using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SocialApp.Application.EventHandlers;
using SocialApp.Application.Providers.Service;
using SocialApp.Application.Registrations.Auth;
using SocialApp.Application.Services;
using SocialApp.Domain.Events;
using SocialApp.Domain.Parameters;

namespace SocialApp.Application.Registrations;

public static class BusinessServiceRegistration
{
    public static IServiceCollection AddBusinessService(this IServiceCollection services, IConfiguration configuration)
    {
        services.ValidatorAssembler();

        LoggerServiceRegistration.SeriLogConfiguration();

        services.AddAuthService(configuration);

        services.AddCacheServices(configuration);

        services.AddHostedService<TokenCleanupService>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(
                typeof(CommentEventHandler).Assembly
    ));

        ServiceRegistrationProvider.RegisterServices(services);

        return services;
    }
}