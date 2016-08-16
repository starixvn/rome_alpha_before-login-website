// -----------------------------------------------------------------------
// <copyright file="UnderMainternanceService.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Fanex.Alpha.Before.Common.Configurations.UnderMainternance;
using Fanex.Alpha.Before.Common.Models;
using Fanex.Alpha.Before.Infrastructure.Extensions;
using ONEbook.UM.Models;
using ONEbook.UM.Services;
using ONEbook.UM.Services.Impl;

namespace Fanex.Alpha.Before.Common.Services.UnderMainternance
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class UnderMainternanceService : IUnderMainternanceService
    {
        private readonly IMaintenanceService _maintenanceService;

        public UnderMainternanceService() : this(null)
        {
        }

        public UnderMainternanceService(IMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService ?? new MaintenanceService();
        }

        /// <summary>
        /// Checking UM.
        /// </summary>
        /// <returns>Return UM info.</returns>
        public UnderMaintenanceModel UnderMainternanceInfo()
        {
            UnderMaintenanceModel result = new UnderMaintenanceModel()
            {
                IsUM = false
            };

            try
            {
                if (_maintenanceService.IsUnderMaintenance())
                {
                    UnderMaintenanceTime underMaintenanceTime = _maintenanceService.GetUnderMaintenanceTime();
                    result.IsUM = true;
                    result.From = underMaintenanceTime.From;
                    result.To = underMaintenanceTime.To;
                }
            }
            catch (Exception ex)
            {
                LogExtension.ExceptionHandler(ex);
            }

            return result;
        }
    }
}
