// -----------------------------------------------------------------------
// <copyright file="UnderMainternanceService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using ONEbook.UM.Models;
using ONEbook.UM.Services;
using ONEbook.UM.Services.Impl;

namespace Fanex.Alpha.Before.Common.Configurations.UnderMainternance
{
    using System;
    using Fanex.Alpha.Before.Common.Models;
    using Fanex.Alpha.Before.Infrastructure.Extensions;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class UnderMainternanceService : IUnderMainternanceService
    {
        private IMaintenanceService _maintenanceService;

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
                    if (_maintenanceService.IsPassUnderMaintenance())
                    {
                        return result;
                    }

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
