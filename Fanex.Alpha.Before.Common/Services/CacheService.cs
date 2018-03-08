namespace Fanex.Alpha.Before.Common.Services
{
    using System;
    using Infrastructure.Extensions;

    public interface ICacheService
    {
        T Get<T>(string key);

        void Set(string key, object value);

        void Set(string key, object value, TimeSpan validFor);
    }

    public class CacheService : ICacheService
    {
        public T Get<T>(string key) => CacheExtension.Get<T>(key);

        public void Set(string key, object value) => CacheExtension.Set(key, value);

        public void Set(string key, object value, TimeSpan validFor) => CacheExtension.Set(key, value, validFor);
    }
}
