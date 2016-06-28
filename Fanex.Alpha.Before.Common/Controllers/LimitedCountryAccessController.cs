// -----------------------------------------------------------------------
// <copyright file="LimitedCountryAccessController.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Alpha.Before.Common.Controllers
{
    using System.Web.Mvc;

    /// <summary>
    /// Limited Country Access.
    /// </summary>
    public class LimitedCountryAccessController : Controller
    {
        /// <summary>
        /// Index page.
        /// </summary>
        /// <returns>Return error page.</returns>
        public ActionResult Index()
        {
            return View("~/Views/Common/LimitedCountryAccess/Index.cshtml");
        }
    }
}
