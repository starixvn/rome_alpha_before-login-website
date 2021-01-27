// --------------------------------------------------------------------------
// <copyright file="Global.asax.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.Alpha.Before.Web.View
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Web.Routing;
    using CaptchaMvc.Infrastructure;
    using CaptchaMvc.Interface;
    using Common.Configurations;
    using Common.Services.LimitedCountryAccessService;
    using Fanex.Logging.Extensions.Database;
    using global::Alpha.Logging;
    using Logging;
    using Logging.Extensions.Web;
    using ONEbook.UM.Configurations;
    using ONEbook.UM.Models;
    using ONEbook.UM.Services.Impl;
    using System.Web.Hosting;

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

            HandleUMError();

            InitIPsCache();
        }

        private static void HandleUMError()
        {
            try
            {
                UMClientManager.WhenError((message) =>
                {
                    ExceptionHandler.Handle(new UMClientException(message));
                });
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }
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
                var maintenanceService = new MaintenanceService();
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