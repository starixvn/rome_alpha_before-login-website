// --------------------------------------------------------------------------
// <copyright file="EnumerableExtension.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.Alpha.Before.Infrastructure.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Enumerable Extension.
    /// </summary>
    public static class EnumerableExtension
    {
        [DebuggerStepThrough]
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (T item in enumerable)
            {
                action(item);
            }
        }
    }
}