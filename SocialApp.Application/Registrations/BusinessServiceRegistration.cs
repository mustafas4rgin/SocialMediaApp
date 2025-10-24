using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using SocialApp.Application.Providers.Service;

namespace SocialApp.Application.Registrations;

public static class BusinessServiceRegistration
{
    public static IServiceCollection AddBusinessService(this IServiceCollection services)
    {
        services.ValidatorAssembler();

        LoggerServiceRegistration.SeriLogConfiguration();

        ServiceRegistrationProvider.RegisterServices(services);
        
        return services;
    }
}