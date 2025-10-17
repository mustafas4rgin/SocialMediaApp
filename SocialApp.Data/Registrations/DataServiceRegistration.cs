using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SocialApp.Data.Contexts;
using SocialApp.Data.Repositories;
using SocialApp.Domain.Contracts;

namespace SocialApp.Data.Registrations;

public static class DataServiceRegistration
{
    public static IServiceCollection AddDataServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddTransient<IGenericRepository,GenericRepository>();

        return services;
    }
}