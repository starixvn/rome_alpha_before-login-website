// --------------------------------------------------------------------------
// <copyright file="CacheExtension.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.Alpha.Before.Infrastructure.Extensions
{
    using Fanex.Caching;
    using Fanex.Caching.Redis;
    using StackExchange.Redis;
    using System;
    using System.Configuration;

    /// <summary>
    /// Caching service.
    /// </summary>
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

        public static T Get<T>(string key)
        {
            return cacheService.Get<T>(key);
        }

        public static void Set<T>(string key, T value)
        {
            cacheService.Set(key, value, new CacheItemOptions
            {
                SlidingExpiration = TimeSpan.FromDays(1)
            });
        }

        public static void Remove(string key)
        {
            cacheService.Remove(key);
        }
    }
}