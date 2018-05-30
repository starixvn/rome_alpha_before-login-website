// --------------------------------------------------------------------------
// <copyright file="Global.asax.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.Alpha.Before.Web.View
{
    using CaptchaMvc.Infrastructure;
    using CaptchaMvc.Interface;
    using Common.Configurations;
    using Common.Services.LimitedCountryAccessService;
    using Logging.Extensions.Database;
    using Logging.Extensions.Web;
    using ONEbook.UM.Configurations;
    using ONEbook.UM.Models;
    using ONEbook.UM.Services.Impl;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Web.Mvc;
    using System.Web.Routing;

    /// <summary>
    /// MVC Application.
    /// For instructions on enabling IIS6 or IIS7 classic mode,
    /// visit http://go.microsoft.com/?LinkId=9394801. 
    /// </summary>
    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        /// <summary>
        /// Registers routes.
        /// </summary>
        /// <param name="routes">The routes.</param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            var defaultParameter = new
            {
                area = "Customer",
                controller = "SignIn",
                action = "Index",
                id = UrlParameter.Optional
            };

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                 defaultParameter);
        }

        /// <summary>
        /// Starts application.
        /// </summary>
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            Fanex.Data.Configuration.DbSettingProviderManager.Current.Start();

            CaptchaUtils.CaptchaManager.StorageProvider = new CookieStorageProvider();
            var defaultCaptchaManager = (DefaultCaptchaManager)CaptchaUtils.CaptchaManager;
            defaultCaptchaManager.ImageUrlFactory = (helper, pair) => ImageUrlFactory(defaultCaptchaManager, helper, pair);
            defaultCaptchaManager.RefreshUrlFactory = RefreshUrlFactory;

            MvcHandler.DisableMvcResponseHeader = true;

            var loggingConfiguration = new LoggingConfiguration()
               .HideSensitiveInfo(GetRegexToHideSensitiveInfo())
               .UseGetDbExceptionInfo(DatabaseInfoGetters.GetDbExceptionInfo);

            ExceptionHandler.UseConfig(loggingConfiguration);

            UMClientManager.WhenError((message) =>
            {
                ExceptionHandler.Handle(new UMClientException(message));
            });

            InitIPsCache();
        }

        private string RefreshUrlFactory(UrlHelper urlHelper, KeyValuePair<string, ICaptchaValue> keyValuePair)
        {
            return urlHelper.Action("Refresh", "DefaultCaptcha", new { area = string.Empty });
        }

        /// <summary>
        /// Image URL Factory.
        /// </summary>
        /// <param name="captchaManager">Captcha Manager.</param>
        /// <param name="urlHelper">URL Helper.</param>
        /// <param name="keyValuePair">Key Value pair.</param>
        /// <returns>Return Result.</returns>
        private static string ImageUrlFactory(DefaultCaptchaManager captchaManager, UrlHelper urlHelper, KeyValuePair<string, ICaptchaValue> keyValuePair)
        {
            return urlHelper.Action(
                "Generate",
                "DefaultCaptcha",
                new RouteValueDictionary
                                {
                                    {
                                        "area", string.Empty
                                    },
                                    {
                                        captchaManager.TokenParameterName, keyValuePair.Key
                                    }
                                });
        }

        protected void Application_Error()
        {
            if (Request.IsLocal)
            {
                return;
            }

            ExceptionHandler.Handle(Server.GetLastError());
        }

        protected void Application_PreSendRequestHeaders()
        {
            Response.Headers.Remove("Server");
            Response.Headers.Remove("X-AspNet-Version");
            Response.Headers.Remove("X-AspNetMvc-Version");
        }

        private static List<string> GetRegexToHideSensitiveInfo()
        {
            var result = new List<string>();
            var sensitiveInfo = new[] { "password", "txtpassword" };

            foreach (var info in sensitiveInfo)
            {
                result.Add($"(?:{info}=)(?<paramValue>[^&|^\\s]+)");
                result.Add($"(\"{info}\":)(?<paramValue>[^,|^\r]+)");
            }

            return result;
        }
        
        private static void InitIPsCache()
        {
            try
            {
                var maintenanceService = new MaintenanceService();
                if (!maintenanceService.IsUnderMaintenance())
                {
                    var activeRestrictedCountry = new LimitedCountriesConfigService().GetActiveRestrictedCountriesConfig();
                    new LimitedCountryAccessService(AppSetting.TrackRunTime).GetIPRangesByRestrictedCountry(activeRestrictedCountry, isIpV6: false);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, TraceEventType.Information);
            }
        }
    }
}