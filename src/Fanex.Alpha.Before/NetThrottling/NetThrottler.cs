using Fanex.Alpha.Before.NetThrottling.Models;
using Fanex.Alpha.Before.NetThrottling.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace Fanex.Alpha.Before.NetThrottling;

public interface INetThrottler
{
    Task<TResult> Throttle<TResult>(
            Func<Func<TResult, TResult>, Task<TResult>> work,
            Func<TResult> blocked,
            ThrottlePolicy policy,
            CacheIdentity cacheIdentity);
}

public class NetThrottler : INetThrottler
{
    private readonly IMemoryCache _memoryCache;

    public NetThrottler(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public async Task<TResult> Throttle<TResult>(Func<Func<TResult, TResult>, Task<TResult>> work, Func<TResult> blocked, ThrottlePolicy policy, CacheIdentity cacheIdentity)
    {
        TResult success(TResult returnType)
        {
            var throttleResult = Throttle(policy, cacheIdentity);
            if (throttleResult == ThrottleResult.Blocked)
            {
                AddBlacklist(policy, cacheIdentity);
                RemoveThrottling(cacheIdentity);
                return blocked();
            }

            return returnType;
        }

        if (cacheIdentity == null)
        {
            return await work(success);
        }

        if (IsBlacklist(cacheIdentity))
        {
            return blocked();
        }

        return await work(success);
    }

    public ThrottleResult Throttle(ThrottlePolicy policy, CacheIdentity cacheIdentity)
    {
        return Throttle(policy, cacheIdentity, false);
    }

    public ThrottleResult Throttle(ThrottlePolicy policy, CacheIdentity cacheIdentity, bool blockImmediately)
    {
        if (cacheIdentity == null)
        {
            return ThrottleResult.Allowed;
        }

        if (IsBlacklist(cacheIdentity))
        {
            return ThrottleResult.Blocked;
        }

        var throttleCounter = _memoryCache.Get<ThrottleCounter>(cacheIdentity.ThrottleKey);

        throttleCounter = throttleCounter == null || Clock.UtcNow > throttleCounter.Timestamp + TimeSpan.FromSeconds(policy.ThrottleDuration)
            ? new ThrottleCounter
            {
                Timestamp = Clock.UtcNow,
                TotalRequests = 1
            }
            : new ThrottleCounter
            {
                Timestamp = throttleCounter.Timestamp,
                TotalRequests = throttleCounter.TotalRequests + 1
            };

        if (throttleCounter.TotalRequests > policy.MaxRequests ||
            (blockImmediately && throttleCounter.TotalRequests == policy.MaxRequests))
        {
            AddBlacklist(policy, cacheIdentity);
            RemoveThrottling(cacheIdentity);
            return ThrottleResult.Blocked;
        }

        _memoryCache.Set(cacheIdentity.ThrottleKey, throttleCounter, new MemoryCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromSeconds(policy.ThrottleDuration)
        });
        return ThrottleResult.Allowed;
    }

    public bool IsBlacklist(CacheIdentity cacheIdentity)
    {
        return _memoryCache.TryGetValue(cacheIdentity.BlockKey, out _);
    }

    public void RemoveThrottling(CacheIdentity cacheIdentity)
    {
        _memoryCache.Remove(cacheIdentity.ThrottleKey);
    }

    public void AddBlacklist(ThrottlePolicy policy, CacheIdentity cacheIdentity)
    {
        _memoryCache.Set(cacheIdentity.BlockKey, Clock.UtcNow, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(policy.BlockDuration)
        });
    }
}