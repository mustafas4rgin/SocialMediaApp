using SocialApp.Domain.Parameters;

namespace SocialApp.Application.Helpers;

public static class ListHelper
{
    public static string UsersListKey(QueryParameters p)
    => $"user:list:v1:q={StringHelper.Sha256Base64Url(StringHelper.Normalize(p.Search))}";
    public static string PostsListKey(QueryParameters p)
    => $"post:list:v1:q={StringHelper.Sha256Base64Url(StringHelper.Normalize(p.Search))}";
    //likes
    public static string LikesListKey(QueryParameters p) =>
        "likes:list";

    public static string LikesByUserKey(int userId, QueryParameters p) =>
        $"likes:user:{userId}";

    public static string LikesByPostKey(int postId, QueryParameters p) =>
        $"likes:post:{postId}";

    public static string LikeCountByPostKey(int postId) =>
        $"likes:count:post:{postId}";
}
