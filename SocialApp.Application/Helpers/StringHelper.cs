namespace SocialApp.Application.Helpers;

public static class StringHelper
{
    public static string Normalize(string? s) => string.IsNullOrWhiteSpace(s) ? "" : s.Trim().ToLowerInvariant();

}