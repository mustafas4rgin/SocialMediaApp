using Microsoft.Extensions.DependencyInjection;

namespace SocialApp.Application.Registrations;

public static class ValidatorServiceAssembler
{
    public static IServiceCollection ValidatorAssembler(this IServiceCollection services)
    {

        services.AddEntityValidators();

        services.AddCreateDtoValidators();

        services.AddUpdateDtoValidators();
        
        return services;
    }
}