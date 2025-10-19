using Microsoft.Extensions.DependencyInjection;

namespace SocialApp.Application.Registrations;

public static class BusinessServiceRegistration
{
    public static IServiceCollection AddBusinessService(this IServiceCollection services)
    {
        services.ValidatorAssembler();

        return services;
    }
}