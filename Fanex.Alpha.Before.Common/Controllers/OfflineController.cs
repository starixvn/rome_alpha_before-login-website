// -----------------------------------------------------------------------
// <copyright file="OfflineController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Alpha.Before.Common.Controllers
{
    using System.Web.Mvc;
    using System.Web.Security;
    using Fanex.Alpha.Before.Common.Configurations.UnderMainternance;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class OfflineController : Controller
    {
        private IUnderMainternanceService _mainternanceService;

        public OfflineController() :
            this(null)
        {
        }

        public OfflineController(UnderMainternanceService mainternanceService)
        {
            _mainternanceService = mainternanceService ?? new UnderMainternanceService();
        }

        /// <summary>
        /// Checking down time.
        /// </summary>
        /// <returns>Return result.</returns>
        [HttpGet]
        public ActionResult Index()
        {
            var downTime = _mainternanceService.UnderMainternanceInfo();
            if (!downTime.IsUM)
            {
                if (Request.QueryString["UMToken"] != null)
                {
                    Session["UMToken"] = Request.QueryString["UMToken"];
                }

                FormsAuthentication.RedirectToLoginPage();
            }

            ViewBag.UmtFrom = downTime.From;
            ViewBag.UmtTo = downTime.To;

            return View("~/Views/Common/UnderMaintenance/Index.cshtml");
        }
    }
}
