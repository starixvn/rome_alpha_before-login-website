// --------------------------------------------------------------------------
// <copyright file="SessionExtension.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.Alpha.Before.Infrastructure.Extensions
{
    using System;
    using System.Web;
    using Logging.Extensions.Web;

    /// <summary>
    /// Session Extension.
    /// </summary>
    public static class SessionExtension
    {
        /// <summary>
        /// Set Value to session with specific type.
        /// </summary>
        /// <typeparam name="T">Parameter type.</typeparam>
        /// <param name="sessionKey">Session key.</param>
        /// <param name="value">Return session value.</param>
        public static void Set<T>(string sessionKey, T value)
        {
            try
            {
                HttpContext.Current.Session[sessionKey] = value;
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }
        }

        /// <summary>
        /// Get Value from Session with specific Type.
        /// </summary>
        /// <typeparam name="T">Parameter type.</typeparam>
        /// <param name="sessionKey">Session key.</param>
        /// <returns>Return session value.</returns>
        public static T Get<T>(string sessionKey)
        {
            try
            {
                T value = (T)HttpContext.Current.Session[sessionKey];
                return value;
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
                return default(T);
            }
        }

        /// <summary>
        /// Set object to Session.
        /// </summary>
        /// <param name="sessionKey">Session key.</param>
        /// <param name="value">Return result.</param>
        public static void Set(string sessionKey, object value)
        {
            SessionExtension.Set<object>(sessionKey, value);
        }

        /// <summary>
        /// Get value object from Session.
        /// </summary>
        /// <param name="sessionKey">Session key.</param>
        /// <returns>Return result.</returns>
        public static object Get(string sessionKey)
        {
            return SessionExtension.Get<object>(sessionKey);
        }
    }
}
