// -----------------------------------------------------------------------
// <copyright file="UnderMaintenanceModel.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Alpha.Before.Common.Models
{
    /// <summary>
    /// Under maintenance model.
    /// </summary>
    public class UnderMaintenanceModel
    {
        public bool IsUM
        {
            get;
            set;
        }

        public string From
        {
            get;
            set;
        }

        public string To
        {
            get;
            set;
        }

        public bool HasError { get; set; }
    }
}