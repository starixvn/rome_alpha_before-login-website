// --------------------------------------------------------------------------
// <copyright file="SystemTime.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.Alpha.Before.Infrastructure.Helpers
{
    using System;

    /// <summary>
    /// System Time.
    /// </summary>
    public static class SystemTime
    {
        public static Func<DateTime> Now
        {
            get
            {
                return () => DateTime.UtcNow;
            }
        }
    }
}