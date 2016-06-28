// --------------------------------------------------------------------------
// <copyright file="TimeInfo.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.Alpha.Before.Infrastructure.Models
{
    using System;

    /// <summary>
    /// Time Info.
    /// </summary>
    [Serializable]
    public class TimeInfo
    {
        public int Days { get; set; }

        public int Hours { get; set; }

        public int Minutes { get; set; }

        public int Seconds { get; set; }
    }
}
