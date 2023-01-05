using System.Diagnostics;

namespace Fanex.Alpha.Before.Helpers
{
    public static class Converter
    {
        /// <summary>
        /// If object type in T is null or DBNull.Value, return default(T).
        /// </summary>
        /// <typeparam name="T">Type of converted value.</typeparam>
        /// <param name="item">The object item.</param>
        /// <returns>T or default(T).</returns>
        [DebuggerStepThrough]
        public static T To<T>(object item)
        {
            if (!ReferenceEquals(item, null) && item != DBNull.Value)
            {
                return (T)Convert.ChangeType(item, typeof(T));
            }

            return default(T);
        }

        /// <summary>
        ///   <see cref="ToBool(object item, bool defaultValue = false)"/>
        /// </summary>
        /// <param name="item">The object item.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The result.</returns>
        [DebuggerStepThrough]
        public static int ToInt(object item, int defaultValue = 0)
        {
            int result = defaultValue;
            bool b = false;

            if (!ReferenceEquals(item, null) && item != DBNull.Value)
            {
                b = int.TryParse(item.ToString(), out result);
            }

            return b ? result : defaultValue;
        }

        /// <summary>
        /// Convert object to Boolean
        /// If object is null or DBNull.Value then return defaultValue.
        /// </summary>
        /// <param name="item">The object item.</param>
        /// <param name="defaultValue">If set to <c>true</c> [default value].</param>
        /// <returns>The result.</returns>
        [DebuggerStepThrough]
        public static bool ToBool(object item, bool defaultValue = false)
        {
            bool result = defaultValue;
            bool b = false;

            if (!ReferenceEquals(item, null) && item != DBNull.Value)
            {
                b = bool.TryParse(item.ToString(), out result);
            }

            return b ? result : defaultValue;
        }

        /// <summary>
        /// Return DateTime.MinValue if item is null or DBNull.Value.
        /// </summary>
        /// <param name="item">The object item.</param>
        /// <returns>The result.</returns>
        [DebuggerStepThrough]
        public static DateTime ToDateTime(object item)
        {
            return ToDateTime(item, DateTime.MinValue);
        }

        /// <summary>
        ///   <see cref="ToBool(object item, bool defaultValue = false)"/>
        /// </summary>
        /// <param name="item">The object item.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The result.</returns>
        public static DateTime ToDateTime(object item, DateTime defaultValue)
        {
            DateTime result = defaultValue;
            bool b = false;

            if (!ReferenceEquals(item, null) && item != DBNull.Value)
            {
                b = DateTime.TryParse(item.ToString(), out result);
            }

            return b ? result : defaultValue;
        }

        /// <summary>
        /// If object item in type T equal null, return string empty.
        /// </summary>
        /// <param name="item">The object item.</param>
        /// <returns>The result.</returns>
        [DebuggerStepThrough]
        public static string ToString2(object item)
        {
            if (!ReferenceEquals(item, null) && item != DBNull.Value)
            {
                return item.ToString();
            }

            return string.Empty;
        }
    }
}