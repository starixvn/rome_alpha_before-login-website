using Fanex.Alpha.Before.NetThrottling.Models;
using Fanex.Alpha.Before.NetThrottling.Utils;
using Fanex.Caching;
using System.Text.Json;

namespace Fanex.Alpha.Before.NetThrottling;

public interface INetThrottler
{
    TResult Throttle<TResult>(
            Func<Func<TResult, TResult>, TResult> work,
            Func<TResult> blocked,
            ThrottlePolicy policy,
            CacheIdentity cacheIdentity);
}

public class NetThrottler : INetThrottler
{
    private readonly ICacheService cacheService;

    public NetThrottler(ICacheService cacheService)
    {
        this.cacheService = cacheService;
    }

    public TResult Throttle<TResult>(Func<Func<TResult, TResult>, TResult> work, Func<TResult> blocked, ThrottlePolicy policy, CacheIdentity cacheIdentity)
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
            return work(success);
        }

        if (IsBlacklist(cacheIdentity))
        {
            return blocked();
        }

        return work(success);
    }

    public ThrottleResult Throttle(ThrottlePolicy policy, CacheIdentity cacheIdentity)
    {
        return Throttle(policy, cacheIdentity, false);
    }

    public ThrottleResult Throttle(ThrottlePolicy policy, CacheIdentity cacheIdentity, bool blockImmediately)
    {
        if (cacheIdentity == null)
            return ThrottleResult.Allowed;
        if (IsBlacklist(cacheIdentity))
        {
            return ThrottleResult.Blocked;
        }

        var jsonString = cacheService.Get<string>(cacheIdentity.ThrottleKey);
        var throttleCounter = !string.IsNullOrWhiteSpace(jsonString) ? JsonSerializer.Deserialize<ThrottleCounter>(jsonString) : null;

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

        cacheService.Set(
            cacheIdentity.ThrottleKey,
            JsonSerializer.Serialize(throttleCounter), new CacheItemOptions
            {
                SlidingExpiration = TimeSpan.FromSeconds(policy.ThrottleDuration)
            });
        return ThrottleResult.Allowed;
    }

    public bool IsBlacklist(CacheIdentity cacheIdentity)
    {
        return cacheService.Contain(cacheIdentity.BlockKey);
    }

    public void RemoveThrottling(CacheIdentity cacheIdentity)
    {
        cacheService.Remove(cacheIdentity.ThrottleKey);
    }

    public void AddBlacklist(ThrottlePolicy policy, CacheIdentity cacheIdentity)
    {
        cacheService.Set(
            cacheIdentity.BlockKey,
            Clock.UtcNow, new CacheItemOptions
            {
                RelativeExpiration = TimeSpan.FromSeconds(policy.BlockDuration)
            });
    }
}