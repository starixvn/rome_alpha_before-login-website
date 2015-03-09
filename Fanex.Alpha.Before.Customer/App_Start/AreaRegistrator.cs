// --------------------------------------------------------------------------
// <copyright file="AreaRegistrator.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.Alpha.Before.Customer.App_Start
{
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Optimization;

    /// <summary>
    /// Area Register.
    /// </summary>
    public class AreaRegistrator : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Customer";
            }
        }

        /// <summary>
        /// Register method.
        /// </summary>
        /// <param name="config">HTTP configuration.</param>
        public static void Register(HttpConfiguration config)
        {
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {            
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(this.AreaName, context);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Register(System.Web.Http.GlobalConfiguration.Configuration);            
        }
    }
}