using SocialApp.Domain.Parameters;

namespace SocialApp.Application.Helpers;

public static class ListHelper
{
    public static string UsersListKey(QueryParameters p)
    => $"user:list:v1:inc={StringHelper.Normalize(p.Include)}:q={StringHelper.Sha256Base64Url(StringHelper.Normalize(p.Search))}";
    public static string PostsListKey(QueryParameters p)
    => $"post:list:v1:inc={StringHelper.Normalize(p.Include)}:q={StringHelper.Sha256Base64Url(StringHelper.Normalize(p.Search))}";
    public static string LikesListKey(QueryParameters p)
    => $"role:list:v1:inc={StringHelper.Normalize(p.Include)}:q={StringHelper.Sha256Base64Url(StringHelper.Normalize(p.Search))}";
}