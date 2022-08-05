namespace Fanex.Alpha.Before.Infrastructure.Extensions
{
    using System;
    using System.Configuration;
    using Fanex.Caching;
    using Fanex.Caching.Redis;
    using Fanex.Logging.Extensions.Web;
    using StackExchange.Redis;

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

        public static ICacheService CacheService => cacheService;

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
            Set<T>(key, value, TimeSpan.FromDays(1));
        }

        public static void Set<T>(string key, T value, CacheItemOptions options)
        {
            try
            {
                cacheService.Set(key, value, options);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }
        }

        public static void Set<T>(string key, T value, TimeSpan timeSpan)
        {
            Set<T>(key, value, new CacheItemOptions
            {
                SlidingExpiration = timeSpan
            });
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

        public static bool Contain(string key)
        {
            return cacheService.Contain(key);
        }
    }
}