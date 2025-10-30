using System.Collections.Concurrent;

namespace SocialApp.Application.Helpers;

public static class SemaphoreSlimHelper
{
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

    public static async Task LockAsync(string key, CancellationToken ct = default)
    {
        var sem = _locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
        await sem.WaitAsync(ct);
    }

    public static void Release(string key)
    {
        if (_locks.TryGetValue(key, out var sem))
        {
            sem.Release();
            if (sem.CurrentCount == 1)
                _locks.TryRemove(key, out _);
        }
    }
}
