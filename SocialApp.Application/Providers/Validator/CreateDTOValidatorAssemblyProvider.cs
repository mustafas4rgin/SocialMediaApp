using SocialApp.Application.Validators.DTO;
using SocialApp.Application.Validators.DTO.Create;

namespace SocialApp.Application.Providers.Validator;

public class CreateDTOValidatorAssemblyProvider
{
    public static Type[] GetValidatorAssemblies()
    {
        return new[]
        {
            typeof(CreateCommentDTOValidator),
            typeof(CreateCommentResponseDTOValidator),
            typeof(CreateFollowDTOValidator),
            typeof(CreateLikeDTOValidator),
            typeof(CreatePostBrutalDTOValidator),
            typeof(CreatePostDTOValidator),
            typeof(CreatePostImageDTOValidator),
            typeof(CreateRoleDTOValidator),
            typeof(CreateUserDTOValidator),
            typeof(CreateUserImageDTOValidator),
        };
    }
}