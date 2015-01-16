// --------------------------------------------------------------------------
// <copyright file="CacheExtension.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.Alpha.Before.Infrastructure.Extensions
{
    using System;
    using Fanex.Caching;

    /// <summary>
    /// Caching service.
    /// </summary>
    public class CacheExtension
    {
        private static ICache cache = null;

        public CacheExtension() : this(null)
        {
        }

        public CacheExtension(string catcheType)
        {
            cache = Caching.CacheService.CreateInstance(!string.IsNullOrEmpty(catcheType) ? catcheType : CacheType.MemoryCache.ToString());
        }

        /// <summary>
        /// Getting cache value.
        /// </summary>
        /// <param name="key">Cache key.</param>
        /// <returns>Return value.</returns>
        public static object Get(string key)
        {
            return cache.Get<object>(key);
        }

        /// <summary>
        /// Setting cache value.
        /// </summary>
        /// <param name="key">Cache key.</param>
        /// <param name="value">The value that saving to cache.</param>
        public static void Set(string key, object value)
        {
            cache.Set(key, value);
        }

        /// <summary>
        /// Getting cache value.
        /// </summary>
        /// <typeparam name="T">Parameter type.</typeparam>
        /// <param name="key">Cache key.</param>
        /// <returns>Return value.</returns>
        public static T Get<T>(string key)
        {
            return cache.Get<T>(key);
        }

        /// <summary>
        /// Setting cache value.
        /// </summary>
        /// <typeparam name="T">Parameter type.</typeparam>
        /// <param name="key">Cache key.</param>
        /// <param name="value">Return value.</param>
        public static void Set<T>(string key, T value)
        {
            cache.Set<T>(key, value);
        }

        /// <summary>
        /// Setting cache value.
        /// </summary>
        /// <param name="key">Cache key.</param>
        /// <param name="value">The value that saving to cache.</param>
        /// <param name="expire">Expire time.</param>
        public static void Set(string key, object value, TimeSpan expire)
        {
            cache.Set(key, value, expire);
        }

        /// <summary>
        /// Setting cache value.
        /// </summary>
        /// <typeparam name="T">Parameter type.</typeparam>
        /// <param name="key">Cache key.</param>
        /// <param name="value">The value that saving to cache.</param>
        /// <param name="expire">Expire time.</param>
        public static void Set<T>(string key, T value, TimeSpan expire)
        {
            cache.Set<T>(key, value, expire);
        }

        /// <summary>
        /// Removing the cache.
        /// </summary>
        public static void Delete()
        {
            cache.Clear();
        }

        /// <summary>
        /// Deletes the specified key.
        /// </summary>
        /// <param name="key">The key to remove.</param>
        public static void Delete(string key)
        {
            cache.Remove(key);
        }
    }
}
