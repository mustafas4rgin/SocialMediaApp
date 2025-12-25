using SocialApp.Domain.Entities;

namespace SocialApp.Application.Helpers;

public static class GetById
{
    public static string Role(int id)
    => $"role:item:{id}";

    public static string Post(int id)
    => $"post:item:{id}";

    public static string User(int id)
    => $"user:item:{id}";
    public static string Like(int id)
    => $"like:item:{id}";
}
