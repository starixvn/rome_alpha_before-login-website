namespace Fanex.Alpha.Before.Infrastructure.Extensions
{
    using Fanex.Caching;
    using Fanex.Caching.Redis;
    using Fanex.Logging.Extensions.Web;
    using StackExchange.Redis;
    using System;
    using System.Configuration;

    public static class CacheExtension
    {
        private static readonly ICacheService cacheService = InitCacheService();

        private static ICacheService InitCacheService()
        {
            var redisConfigurationString = ConfigurationManager.AppSettings["RedisConfigurationOptions:Alpha.Before"];
            var configurationOptions = ConfigurationOptions.Parse(redisConfigurationString);
            var redisCacheOptions = new RedisCacheOptions
            {
                ApplicationName = configurationOptions.ClientName,
                ConnectionOptions = configurationOptions
            };
            return new CacheService(new RedisCache(redisCacheOptions));
        }

        public static T Get<T>(string key, T defaultValue)
        {
            try
            {
                return cacheService.Get<T>(key);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
                return defaultValue;
            }
        }

        public static void Set<T>(string key, T value)
        {
            try
            {
                cacheService.Set(key, value, new CacheItemOptions
                {
                    SlidingExpiration = TimeSpan.FromDays(1)
                });
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }
        }

        public static void Remove(string key)
        {
            try
            {
                cacheService.Remove(key);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }
        }
    }
}