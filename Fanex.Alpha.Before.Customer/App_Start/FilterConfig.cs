// --------------------------------------------------------------------------
// <copyright file="FilterConfig.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.Alpha.Before.Customer.App_Start
{
    using System.Web.Mvc;

    /// <summary>
    /// Filter Config.
    /// </summary>
    public static class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}