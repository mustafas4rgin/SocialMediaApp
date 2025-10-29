using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SocialApp.Application.Registrations;

public static class CacheServiceRegistration
{
    public static IServiceCollection AddCacheServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = config["Redis:Configuration"];
            options.InstanceName = "SocialApp:";
        });

        return services;
    }
}