// -----------------------------------------------------------------------
// <copyright file="SessionExtenssion.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Razorbill.Sphinx.Core.Extensions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Web;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public class SessionExtension
    {
        /// <summary>
        /// Set Value to session with specific type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sessionKey"></param>
        /// <param name="value"></param>
        public static void Set<T>(string sessionKey, T value)
        {
            try
            {
                HttpContext.Current.Session[sessionKey] = value;
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Get Value from Session with specific Type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public static T Get<T>(string sessionKey)
        {
            try
            {
                T value = (T)HttpContext.Current.Session[sessionKey];
                return value;
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        /// <summary>
        /// Set object to Session
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="value"></param>
        public static void Set(string sessionKey, object value)
        {
            SessionExtension.Set<object>(sessionKey, value);
        }

        /// <summary>
        /// Get value object from Session
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public static object Get(string sessionKey)
        {
            return SessionExtension.Get<object>(sessionKey);
        }
    }
}
