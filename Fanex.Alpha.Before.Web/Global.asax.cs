// --------------------------------------------------------------------------
// <copyright file="Global.asax.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.Alpha.Before.Web.View
{
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
                controller = "Home",
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
        }
    }
}