// -----------------------------------------------------------------------
// <copyright file="ErrorPageController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Alpha.Before.Common.Controllers
{
    using System.Web.Mvc;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ErrorPageController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.ErrorMessage = Fanex.Alpha.Before.Infrastructure.Resources.Labels.Error_Server;
            return View("~/Views/Common/ErrorPage/Index.cshtml");
        }
    }
}