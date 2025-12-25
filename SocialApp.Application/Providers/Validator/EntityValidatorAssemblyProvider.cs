using SocialApp.Application.Validators;

namespace SocialApp.Application.Providers.Validator;

public static class EntityValidatorAssemblyProvider
{
    public static Type[] GetValidatorAssemblies()
    {
        return new[]
        {
            typeof(CommentValidator),
            typeof(CommentResponseValidator),
            typeof(FollowValidator),
            typeof(PostValidator),
            typeof(LikeValidator),
            typeof(RoleValidator),
            typeof(PostBrutalValidator),
            typeof(PostImageValidator),
            typeof(UserValidator),
            typeof(UserImageValidator),
            typeof(NotificationValidator)
        };
    }
}