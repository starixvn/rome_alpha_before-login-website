// -----------------------------------------------------------------------
// <copyright file="CacheExtension.cs" company="Nexcel Solution">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Razorbill.Sphinx.Core.Extensions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Fanex.Caching;

    /// <summary>
    /// TODO: Cache extension.
    /// </summary>
    public class CacheExtension
    {
        private static ICache _cache;

        static CacheExtension()
        {
            _cache = Caching.CacheService.CreateInstance(CacheType.MemoryCache);
        }

        /// <summary>
        /// Get caching.
        /// </summary>
        /// <param name="key">Cache key.</param>
        /// <returns>Return cache value.</returns>
        public static object Get(string key)
        {
            try
            {
                return _cache.Get<object>(key);
            }
            catch (Exception exception)
            {
                LogExtension.ExceptionHandler(exception);
                return null;
            }
        }

        public static void Set(string key, object value)
        {
            try
            {
                _cache.Set(key, value);
            }
            catch (Exception exception)
            {
                LogExtension.ExceptionHandler(exception);
            }
        }

        public static T Get<T>(string key)
        {
            return _cache.Get<T>(key);
        }

        public static void Set<T>(string key, T value)
        {
            try
            {
                _cache.Set<T>(key, value);
            }
            catch (Exception exception)
            {
                LogExtension.ExceptionHandler(exception);
            }
        }

        public static void Set(string key, object value, TimeSpan expire)
        {
            try
            {
                _cache.Set(key, value, expire);
            }
            catch (Exception exception)
            {
                LogExtension.ExceptionHandler(exception);
            }
        }

        public static void Set<T>(string key, T value, TimeSpan expire)
        {
            _cache.Set<T>(key, value, expire);
        }

        public static void Delete(string key)
        {
            _cache.Clear();
        }
    }
}
