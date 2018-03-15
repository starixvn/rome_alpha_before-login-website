namespace Fanex.Alpha.Before.Common.Services
{
    using System;
    using Caching;

    public interface ICacheService
    {
        T GetOrSet<T>(string key, Func<T> factory, TimeSpan validFor);
    }

    public class CacheServiceImpl : ICacheService
    {
        private readonly ICache instance;

        public CacheServiceImpl()
        {
            instance = CacheService.CreateInstance(CacheType.MemoryCache);
        }

        public T GetOrSet<T>(string key, Func<T> factory, TimeSpan validFor)
        {
            var result = instance.Get<T>(key);
            if (result != null)
            {
                return result;
            }

            result = factory();
            instance.Set(key, result, validFor);
            return result;
        }
    }
}
