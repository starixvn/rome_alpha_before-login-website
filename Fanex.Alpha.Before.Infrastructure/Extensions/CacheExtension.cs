// --------------------------------------------------------------------------
// <copyright file="CacheExtension.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.Alpha.Before.Infrastructure.Extensions
{
    using System;
    using Couchbase;
    using Enyim.Caching.Memcached;

    /// <summary>
    /// Caching service.
    /// </summary>
    public static class CacheExtension
    {
        private static CouchbaseClient _couchbaseClient = new CouchbaseClient();

        public static object Get(string key)
        {
            return _couchbaseClient.Get(key);
        }

        public static T Get<T>(string key)
        {
            return _couchbaseClient.Get<T>(key);
        }

        public static void Set(string key, object value)
        {
            _couchbaseClient.ExecuteStore(
                StoreMode.Set,
                key,
                value,
                Couchbase.Operations.PersistTo.One,
                Couchbase.Operations.ReplicateTo.Two);
        }

        public static void Set(string key, object value, TimeSpan validFor)
        {
            _couchbaseClient.ExecuteStore(
                StoreMode.Set,
                key,
                value,
                validFor,
                Couchbase.Operations.PersistTo.One,
                Couchbase.Operations.ReplicateTo.Two);
        }

        public static void Remove(string key)
        {
            _couchbaseClient.Remove(key);
        }
    }
}