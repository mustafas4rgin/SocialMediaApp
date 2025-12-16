using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace SocialApp.Application.Helpers;

public static class CacheHelper
{
    private const string KeysRegistryKey = "cache:keys";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    // ---------------------------
    // TYPED GET / SET
    // ---------------------------

    public static async Task<T?> GetTypedAsync<T>(
        IDistributedCache cache,
        string key,
        CancellationToken ct = default)
    {
        var json = await cache.GetStringAsync(key, ct);

        if (string.IsNullOrEmpty(json))
            return default;

        return JsonSerializer.Deserialize<T>(json, JsonOptions);
    }

    public static async Task SetTypedAsync<T>(
        IDistributedCache cache,
        string key,
        T value,
        CancellationToken ct = default,
        DistributedCacheEntryOptions? options = null)
    {
        if (value is null)
            return;

        options ??= new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
            SlidingExpiration = TimeSpan.FromMinutes(3)
        };

        var json = JsonSerializer.Serialize(value, JsonOptions);

        await cache.SetStringAsync(key, json, options, ct);

        await RegisterKeyAsync(cache, key, ct);
    }
    public static async Task RemoveAsync(
        IDistributedCache cache,
        string key,
        CancellationToken ct = default)
    {
        await cache.RemoveAsync(key, ct);
        await UnregisterKeyAsync(cache, key, ct);
    }
    public static async Task RemoveByPatternAsync(
        IDistributedCache cache,
        string startsWithPattern,
        CancellationToken ct = default)
    {
        var registryJson = await cache.GetStringAsync(KeysRegistryKey, ct);

        if (string.IsNullOrEmpty(registryJson))
            return;

        var keys = JsonSerializer.Deserialize<HashSet<string>>(registryJson, JsonOptions)
                   ?? new HashSet<string>();

        var toRemove = keys
            .Where(k => k.StartsWith(startsWithPattern, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (!toRemove.Any())
            return;

        foreach (var key in toRemove)
        {
            await cache.RemoveAsync(key, ct);
            keys.Remove(key);
        }
        var updatedJson = JsonSerializer.Serialize(keys, JsonOptions);

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12)
        };

        await cache.SetStringAsync(KeysRegistryKey, updatedJson, options, ct);
    }

    private static async Task RegisterKeyAsync(
        IDistributedCache cache,
        string key,
        CancellationToken ct)
    {
        var registryJson = await cache.GetStringAsync(KeysRegistryKey, ct);

        HashSet<string> keys;

        if (string.IsNullOrEmpty(registryJson))
        {
            keys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }
        else
        {
            keys = JsonSerializer.Deserialize<HashSet<string>>(registryJson, JsonOptions)
                   ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        if (!keys.Add(key))
            return;

        var updatedJson = JsonSerializer.Serialize(keys, JsonOptions);

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12)
        };

        await cache.SetStringAsync(KeysRegistryKey, updatedJson, options, ct);
    }

    private static async Task UnregisterKeyAsync(
        IDistributedCache cache,
        string key,
        CancellationToken ct)
    {
        var registryJson = await cache.GetStringAsync(KeysRegistryKey, ct);

        if (string.IsNullOrEmpty(registryJson))
            return;

        var keys = JsonSerializer.Deserialize<HashSet<string>>(registryJson, JsonOptions)
                   ?? new HashSet<string>();

        if (!keys.Remove(key))
            return;

        var updatedJson = JsonSerializer.Serialize(keys, JsonOptions);

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12)
        };

        await cache.SetStringAsync(KeysRegistryKey, updatedJson, options, ct);
    }
}
