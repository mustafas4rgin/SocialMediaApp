using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;

namespace SocialApp.Data.Helpers;

public static class StringHelper
{
    public static bool EmptyCheck(string? s) => string.IsNullOrWhiteSpace(s);
    public static string[] Includes(string i) => i.Split(',', StringSplitOptions.RemoveEmptyEntries);
    public static string Normalize(string? s) => string.IsNullOrWhiteSpace(s) ? "" : s.Trim().ToLowerInvariant();
    public static string Sha256Base64Url(string input)
    {
        if (string.IsNullOrEmpty(input)) return "_";
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(hash).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }
}