namespace Fanex.Alpha.Before.Web.View
{
    using System;
    using System.Collections.Generic;
    using System.Web.Hosting;
    using System.Web.Mvc;
    using System.Web.Routing;
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
    using global::Alpha.SliderCaptcha.Mvc;
    using global::Alpha.SliderCaptcha.Core;

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

            MvcHandler.DisableMvcResponseHeader = true;

            var sensitiveInfo = new List<string> { "password", "txtpassword" };
            LoggingConfig.Init(sensitiveInfo, DatabaseInfoGetters.GetDbExceptionInfo, $"{GetVirtualAppPath()}/assets/bundles/raven.min.js");
            SliderCaptchaHandler.Use();
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