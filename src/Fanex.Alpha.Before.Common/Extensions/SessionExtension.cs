// --------------------------------------------------------------------------
// <copyright file="SessionExtension.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.Alpha.Before.Common.Extensions
{
    using System.Text.Json;
    using Microsoft.AspNetCore.Http;

    public static class SessionExtension
    {
        public static void Set<T>(this ISession session, string sessionKey, T value)
        {
            session.SetString(sessionKey, JsonSerializer.Serialize(value));
        }

        public static T Get<T>(this ISession session, string sessionKey)
        {
            var value = session.GetString(sessionKey);

            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }
}
