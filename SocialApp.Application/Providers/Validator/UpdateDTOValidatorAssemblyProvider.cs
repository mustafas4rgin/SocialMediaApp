using SocialApp.Application.Validators.DTO.Update;

namespace SocialApp.Application.Providers.Validator;

public static class UpdateDTOValidatorAssemblyProvider
{
    public static Type[] GetValidatorAssemblies()
    {
        return new[]
        {
            typeof(UpdateCommentDTOValidator),
            typeof(UpdateCommentResponseDTOValidator),
            typeof(UpdateFollowDTOValidator),
            typeof(UpdateLikeDTOValidator),
            typeof(UpdatePostBrutalDTOValidator),
            typeof(UpdateRoleDTOValidator),
            typeof(UpdatePostDTOValidator),
            typeof(UpdatePostImageDTOValidator),
            typeof(UpdateUserDTOValidator),
            typeof(UpdateUserImageDTOValidator),
        };
    }
}