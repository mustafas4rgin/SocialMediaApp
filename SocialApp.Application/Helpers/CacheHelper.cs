using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace SocialApp.Application.Helpers;

public static class CacheHelper
{
    private static readonly JsonSerializerOptions _json = new() { WriteIndented = false };
    private static DistributedCacheEntryOptions DefaultCacheOptions => new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15),
        SlidingExpiration = TimeSpan.FromMinutes(3)
    };

    public static async Task<T?> GetTypedAsync<T>(IDistributedCache cache, string key, CancellationToken ct)
    {
        var bytes = await cache.GetAsync(key, ct);
        return bytes is null ? default : JsonSerializer.Deserialize<T>(bytes, _json);
    }

    public static Task SetTypedAsync<T>(IDistributedCache cache, string key, T value, CancellationToken ct) =>
        cache.SetAsync(key, JsonSerializer.SerializeToUtf8Bytes(value, _json), DefaultCacheOptions, ct);
}