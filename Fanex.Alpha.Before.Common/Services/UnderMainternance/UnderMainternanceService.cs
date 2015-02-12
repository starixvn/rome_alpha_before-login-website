// -----------------------------------------------------------------------
// <copyright file="UnderMainternanceService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Alpha.Before.Common.Configurations.UnderMainternance
{
    using System;
    using Fanex.Alpha.Before.Common.Configurations;
    using Fanex.Alpha.Before.Common.Models;
    using Fanex.Alpha.Before.Infrastructure.Extensions;
    using Sunplus.UMControllerLibrary;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class UnderMainternanceService : IUnderMainternanceService
    {
        /// <summary>
        /// Checking UM.
        /// </summary>
        /// <returns>Return UM info.</returns>
        public UnderMaintenanceModel UnderMainternanceInfo()
        {
            var result = new UnderMaintenanceModel()
            {
                IsUM = false
            };

            try
            {
                string startTime = string.Empty, endTime = string.Empty;
                var alphaUMSetting = UMSection.Instance;

                if (alphaUMSetting.IsUMTesting)
                {
                    result.IsUM = true;
                    startTime = alphaUMSetting.UMStartTesting;
                    endTime = alphaUMSetting.UMEndTesting;
                }
                else if (alphaUMSetting.EnableCheckUM)
                {
                    result.IsUM = new UMBusiness().isUnderMaintainence(Sites.Agency, alphaUMSetting.UMAddress, ref startTime, ref endTime);                
                }

                result.From = startTime;
                result.To = endTime;
            }
            catch (Exception ex)
            {
                LogExtension.ExceptionHandler(ex);
            }

            return result;
        }
    }
}
