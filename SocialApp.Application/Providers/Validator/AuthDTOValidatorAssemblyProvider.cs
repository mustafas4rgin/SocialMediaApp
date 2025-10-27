using SocialApp.Application.Validators.DTO.Auth;

namespace SocialApp.Application.Providers.Validator;

public class AuthDTOValidatorAssemblyProvider
{
    public static Type[] GetValidatorAssemblies()
    {
        return new[]
        {
            typeof(LoginDTOValidator),
        };
    }
}