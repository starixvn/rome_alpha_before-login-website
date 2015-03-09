// --------------------------------------------------------------------------
// <copyright file="SystemTime.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
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