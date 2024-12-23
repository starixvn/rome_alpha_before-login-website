using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace Fanex.Alpha.Before.Helpers;

public interface IDistributedCacheHelper
{
    Task<bool> Contain(string key);

    Task<T> Get<T>(string key, T defaultValue);

    Task Set<T>(string key, T value);

    Task Set<T>(string key, T value, DistributedCacheEntryOptions options);

    Task Remove(string key);
}

public class DistributedCacheHelper : IDistributedCacheHelper
{
    private readonly IDistributedCache _cache;
    private readonly Logging.ILogger _logger;

    public DistributedCacheHelper(IDistributedCache cache, Logging.ILogger logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<bool> Contain(string key)
    {
        try
        {
            var encodedCached = await _cache.GetAsync(key);
            return encodedCached != null;
        }
        catch (Exception ex)
        {
            await _logger.WarnAsync(ex.ToString());
            return false;
        }
    }

    public async Task<T> Get<T>(string key, T defaultValue)
    {
        try
        {
            var encodedCached = await _cache.GetAsync(key);
            return encodedCached == null
                ? defaultValue
                : JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(encodedCached));
        }
        catch (Exception ex)
        {
            await _logger.WarnAsync(ex.ToString());
            return defaultValue;
        }
    }

    public Task Set<T>(string key, T value)
    {
        return Set(key, value, TimeSpan.FromDays(1));
    }

    private async Task Set<T>(string key, T value, TimeSpan timeSpan)
    {
        await Set(key, value, new DistributedCacheEntryOptions().SetSlidingExpiration(timeSpan));
    }

    public async Task Set<T>(string key, T value, DistributedCacheEntryOptions options)
    {
        try
        {
            byte[] encodedValue = Encoding.UTF8.GetBytes(JsonSerializer.Serialize<T>(value));
            await _cache.SetAsync(key, encodedValue, options);
        }
        catch (Exception ex)
        {
            await _logger.WarnAsync(ex.ToString());
        }
    }

    public Task Remove(string key)
    {
        try
        {
            return _cache.RemoveAsync(key);
        }
        catch (Exception ex)
        {
            return _logger.WarnAsync(ex.ToString());
        }
    }
}