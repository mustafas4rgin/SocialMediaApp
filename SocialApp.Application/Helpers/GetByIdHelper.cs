using SocialApp.Domain.Entities;

namespace SocialApp.Application.Helpers;

public static class GetById
{
    public static string Role(int id, string include)
    => $"role:item:{include}:{id}";

    public static string Post(int id, string include)
    => $"post:item:{include}:{id}";
}