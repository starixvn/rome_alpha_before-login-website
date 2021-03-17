// --------------------------------------------------------------------------
// <copyright file="Global.asax.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.Alpha.Before.Web.View
{
    using System;
    using System.Collections.Generic;
    using System.Web.Hosting;
    using System.Web.Mvc;
    using System.Web.Routing;
    using CaptchaMvc.Infrastructure;
    using CaptchaMvc.Interface;
    using Common.Configurations;
    using Common.Services.LimitedCountryAccessService;
    using Fanex.Caching;
    using Fanex.Logging.Extensions.Database;
    using global::Alpha.Logging;
    using Logging;
    using Logging.Extensions.Web;
    using ONEbook.UM;
    using ONEbook.UM.Configurations;
    using ONEbook.UM.Helpers;
    using ONEbook.UM.Models;
    using ONEbook.UM.NETFramework;
    using ONEbook.UM.Services;
    using ONEbook.UM.Transport;

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
                controller = "auth",
                action = "index",
                id = UrlParameter.Optional
            };

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
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

            CaptchaUtils.CaptchaManager.StorageProvider = new SessionStorageProvider();
            var defaultCaptchaManager = (DefaultCaptchaManager)CaptchaUtils.CaptchaManager;
            defaultCaptchaManager.ImageUrlFactory = (helper, pair) => ImageUrlFactory(defaultCaptchaManager, helper, pair);
            defaultCaptchaManager.RefreshUrlFactory = RefreshUrlFactory;

            MvcHandler.DisableMvcResponseHeader = true;

            var sensitiveInfo = new List<string> { "password", "txtpassword" };
            LoggingConfig.Init(sensitiveInfo, DatabaseInfoGetters.GetDbExceptionInfo, $"{GetVirtualAppPath()}/assets/bundles/raven.min.js");

            var umConfig = new UMClientConfiguration().Initialize();
            UMClientManager
                .UseConfig(umConfig)
                .UseCacheService(new UMCacheService(new CacheService()))
                .UseRestClient(new UMRestClient(umConfig))
                .UseHttpContext(new HttpContextHelper())
                .AddErrorHandler((message) =>
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


        private static void InitIPsCache()
        {
            try
            {
                var maintenanceService = new MaintenanceService(
                    UMClientManager.RestClient,
                    UMClientManager.CacheService,
                    UMClientManager.UMClientConfigs,
                    UMClientManager.HttpContext);
                if (!maintenanceService.IsUnderMaintenance())
                {
                    var activeRestrictedCountry = new LimitedCountriesConfigService().GetActiveRestrictedCountriesConfig();
                    new LimitedCountryAccessService(AppSetting.TrackRunTime).GetIPRangesByRestrictedCountry(activeRestrictedCountry, isIpV6: false);
                }
            }
            catch (Exception ex)
            {
                LogService.Info(ex.ToString());
            }
        }

        private static string GetVirtualAppPath()
        {
            var virtualAppPath = HostingEnvironment.ApplicationVirtualPath;
            return virtualAppPath.Equals("/") ? string.Empty : virtualAppPath;
        }
    }
}