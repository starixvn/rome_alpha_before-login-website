// -----------------------------------------------------------------------
// <copyright file="UnderMaintenaceService.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Razorbill.Sphinx.Service.Common.UnderMaintenance
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Fanex.Razorbill.Sphinx.Core.Configurations;
    using Fanex.Razorbill.Sphinx.Service.Common.Models;
    using Sunplus.Agent.Model.BL.Definitions;
    using Sunplus.Agent.Model.Services;
    using Fanex.Razorbill.Sphinx.Core.Extensions;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public class UnderMaintenanceService
    {
        private string baseDomain = string.Empty;
        static DateTime _lastTimeCheckUM = DateTime.MinValue;
        static object _locker = new object();
        static bool _isUM = false;

        public UnderMaintenanceService()
        {
            this.baseDomain = WebSettings.BaseDomain;
            string currentDomain = AppSettings.BeforeLoginUrl;
            //// Not include dot
            this.baseDomain = currentDomain.Remove(0, currentDomain.IndexOf('.') + 1);
            this.baseDomain = this.baseDomain.Remove(this.baseDomain.Length - 1, 1);
        }

        public UnderMaintenanceModel CheckUM() 
        {
            
            var result = new UnderMaintenanceModel() { 
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
                        if (!string.IsNullOrEmpty(domainExclude) && this.baseDomain.Contains(domainExclude))
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
