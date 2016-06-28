// --------------------------------------------------------------------------
// <copyright file="PeriodRange.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.Alpha.Before.Infrastructure.Models
{
    using System;

    /// <summary>
    /// Base Model.
    /// </summary>
    [Serializable]
    public class PeriodRange : BaseModel
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string FromDateString { get; set; }

        public string ToDateString { get; set; }
    }
}
