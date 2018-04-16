namespace Fanex.Alpha.Before.Common.Services
{
    using System;
    using System.Collections.Generic;
    using Caching;

    public interface ICacheService
    {
        T Get<T>(string key);

        void Set<T>(string key, T value, TimeSpan validFor);

        T GetOrSet<T>(string key, Func<T> factory, TimeSpan validFor);
    }

    public class CacheServiceImpl : ICacheService
    {
        private readonly ICache instance;

        public CacheServiceImpl()
        {
            instance = CacheService.CreateInstance(CacheType.MemoryCache);
        }

        public T Get<T>(string key) => instance.Get<T>(key);

        public void Set<T>(string key, T value, TimeSpan validFor) => instance.Set(key, value, validFor);

        public T GetOrSet<T>(string key, Func<T> factory, TimeSpan validFor)
        {
            var result = Get<T>(key);
            if (!EqualityComparer<T>.Default.Equals(result, default(T)))
            {
                return result;
            }

            result = factory();
            Set(key, result, validFor);
            return result;
        }
    }
}
