using Fanex.Alpha.Before.Helpers;
using Fanex.Alpha.Before.NetThrottling.Models;
using Fanex.Alpha.Before.NetThrottling.Utils;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Fanex.Alpha.Before.NetThrottling;

public interface INetThrottler
{
    Task<TResult> Throttle<TResult>(
            Func<Func<TResult, Task<TResult>>, Task<TResult>> work,
            Func<TResult> blocked,
            ThrottlePolicy policy,
            CacheIdentity cacheIdentity);
}

public class NetThrottler : INetThrottler
{
    private readonly IDistributedCacheHelper cacheHelper;

    public NetThrottler(IDistributedCacheHelper cacheHelper)
    {
        this.cacheHelper = cacheHelper;
    }

    public async Task<TResult> Throttle<TResult>(Func<Func<TResult, Task<TResult>>, Task<TResult>> work, Func<TResult> blocked, ThrottlePolicy policy, CacheIdentity cacheIdentity)
    {
        async Task<TResult> success(TResult returnType)
        {
            var throttleResult = await Throttle(policy, cacheIdentity);
            if (throttleResult == ThrottleResult.Blocked)
            {
                await AddBlacklist(policy, cacheIdentity);
                await RemoveThrottling(cacheIdentity);
                return blocked();
            }

            return returnType;
        }

        if (cacheIdentity == null)
        {
            return await work(success);
        }

        if (await IsBlacklist(cacheIdentity))
        {
            return blocked();
        }

        return await work(success);
    }

    public Task<ThrottleResult> Throttle(ThrottlePolicy policy, CacheIdentity cacheIdentity)
    {
        return Throttle(policy, cacheIdentity, false);
    }

    public async Task<ThrottleResult> Throttle(ThrottlePolicy policy, CacheIdentity cacheIdentity, bool blockImmediately)
    {
        if (cacheIdentity == null)
        {
            return ThrottleResult.Allowed;
        }

        if (await IsBlacklist(cacheIdentity))
        {
            return ThrottleResult.Blocked;
        }

        var jsonString = await cacheHelper.Get(cacheIdentity.ThrottleKey, string.Empty);
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
            await AddBlacklist(policy, cacheIdentity);
            await RemoveThrottling(cacheIdentity);
            return ThrottleResult.Blocked;
        }

        await cacheHelper.Set(
            cacheIdentity.ThrottleKey,
            JsonSerializer.Serialize(throttleCounter), new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromSeconds(policy.ThrottleDuration)
            });
        return ThrottleResult.Allowed;
    }

    public Task<bool> IsBlacklist(CacheIdentity cacheIdentity)
    {
        return cacheHelper.Contain(cacheIdentity.BlockKey);
    }

    public Task RemoveThrottling(CacheIdentity cacheIdentity)
    {
        return cacheHelper.Remove(cacheIdentity.ThrottleKey);
    }

    public Task AddBlacklist(ThrottlePolicy policy, CacheIdentity cacheIdentity)
    {
        return cacheHelper.Set(
             cacheIdentity.BlockKey,
             Clock.UtcNow, new DistributedCacheEntryOptions
             {
                 AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(policy.BlockDuration)
             });
    }
}