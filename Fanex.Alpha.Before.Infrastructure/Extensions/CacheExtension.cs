// --------------------------------------------------------------------------
// <copyright file="CacheExtension.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.Alpha.Before.Infrastructure.Extensions
{
    using Couchbase;
    using Enyim.Caching.Memcached;

    /// <summary>
    /// Caching service.
    /// </summary>
    public class CacheExtension
    {
        private CouchbaseClient _client;

        public CacheExtension()
        {
            this._client = new CouchbaseClient();
        }

        public object Get(string key)
        {
            return _client.Get(key);
        }

        public T Get<T>(string key)
        {
            return _client.Get<T>(key);
        }

        public void Set(string key, object value)
        {
            _client.ExecuteStore(StoreMode.Set, key, value);
        }

        public void Remove(string key)
        {
            _client.Remove(key);
        }
    }
}