// --------------------------------------------------------------------------
// <copyright file="RouteConfig.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.Alpha.Before.Common.App_Start
{
    using System.Reflection;
    using System.Web.Mvc;

    /// <summary>
    /// Route config class.
    /// </summary>
    public static class RouteConfig
    {
        public static void RegisterRoutes(string areaName, AreaRegistrationContext context)
        {
            var controllerName = Assembly.GetExecutingAssembly().GetName().Name + ".Controllers";
            context.MapRoute(
                areaName + "_Default",
                areaName + "/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { controllerName });
        }
    }
}