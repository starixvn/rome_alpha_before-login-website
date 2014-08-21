// -----------------------------------------------------------------------
// <copyright file="UnderMaintenanceService.cs" company="Nexcel Solution">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Razorbill.Sphinx.Service.Common.UnderMaintenance
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Fanex.Razorbill.Sphinx.Core.Configurations;
    using Fanex.Razorbill.Sphinx.Core.Extensions;
    using Fanex.Razorbill.Sphinx.Service.Common.Models;
    using Sunplus.Agent.Model.BL.Definitions;
    using Sunplus.Agent.Model.Services;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class UnderMaintenanceService
    {        
        private static DateTime _lastTimeCheckUM = DateTime.MinValue;
        private static object _locker = new object();
        private string _baseDomain = string.Empty;

        public UnderMaintenanceService()
        {
            this._baseDomain = WebSettings.BaseDomain;
            string currentDomain = AppSettings.BeforeLoginUrl;
            //// Not include dot
            this._baseDomain = currentDomain.Remove(0, currentDomain.IndexOf('.') + 1);
            this._baseDomain = this._baseDomain.Remove(this._baseDomain.Length - 1, 1);
        }

        /// <summary>
        /// Checking UM.
        /// </summary>
        /// <returns>Return UM info.</returns>
        public UnderMaintenanceModel CheckUM() 
        {
            var result = new UnderMaintenanceModel() 
            { 
                IsUM = false
            };
            
            try
            {
                var domainsExcludeDowntime = WebSettings.DomainsExcludeDowntime;
                bool ignoreUMCheck = false;

                if (!string.IsNullOrEmpty(domainsExcludeDowntime))
                {
                    foreach (string domainExclude in domainsExcludeDowntime.Split(','))
                    {
                        if (!string.IsNullOrEmpty(domainExclude) && this._baseDomain.Contains(domainExclude))
                        {
                            ignoreUMCheck = true;
                            continue;
                        }
                    }
                }

                if (!ignoreUMCheck)
                {
                    string startTime = string.Empty, endTime = string.Empty;

                    result.IsUM = MaintenanceService.GetDownTime(out startTime, out endTime);
                    result.From = startTime;
                    result.To = endTime;
                    _lastTimeCheckUM = DateTime.Now;
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
