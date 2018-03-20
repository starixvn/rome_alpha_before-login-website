// --------------------------------------------------------------------------
// <copyright file="AreaRegistrator.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.Alpha.Before.Common.App_Start
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
                return "Common";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {            
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(this.AreaName, context);
            BundleConfig.RegisterBundles(BundleTable.Bundles);         
        }
    }
}