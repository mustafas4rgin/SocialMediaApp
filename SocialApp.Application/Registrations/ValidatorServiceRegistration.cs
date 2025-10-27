using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SocialApp.Application.Providers.Validator;

namespace SocialApp.Application.Registrations;

public static class ValidatorServiceRegistration
{
    public static IServiceCollection AddEntityValidators(this IServiceCollection services)
    {
        var entityValidatorAssemblies = EntityValidatorAssemblyProvider.GetValidatorAssemblies();

        foreach (var assemblyType in entityValidatorAssemblies)
            services.AddValidatorsFromAssemblyContaining(assemblyType);

        return services;
    }
    public static IServiceCollection AddAuthDtoValidators(this IServiceCollection services)
    {
        var authValidatorAssemblies = AuthDTOValidatorAssemblyProvider.GetValidatorAssemblies();

        foreach (var assemblyType in authValidatorAssemblies)
            services.AddValidatorsFromAssemblyContaining(assemblyType);

        return services;
    }
    public static IServiceCollection AddUpdateDtoValidators(this IServiceCollection services)
    {
        var updateDtoValidatorAssemblies = UpdateDTOValidatorAssemblyProvider.GetValidatorAssemblies();

        foreach (var assemblyType in updateDtoValidatorAssemblies)
            services.AddValidatorsFromAssemblyContaining(assemblyType);

        return services;
    }
    public static IServiceCollection AddCreateDtoValidators(this IServiceCollection services)
    {
        var createDTOValidatorAssemblies = CreateDTOValidatorAssemblyProvider.GetValidatorAssemblies();

        foreach (var assemblyType in createDTOValidatorAssemblies)
            services.AddValidatorsFromAssemblyContaining(assemblyType);

        return services;
    }
}