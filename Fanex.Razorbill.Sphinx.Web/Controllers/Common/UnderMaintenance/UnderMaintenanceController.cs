// -----------------------------------------------------------------------
// <copyright file="UnderMaintenanceController.cs" company="Nexcel Solution">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Razorbill.Sphinx.Web.Controllers.Common.UnderMaintenance
{
    using System.Web.Mvc;
    using Fanex.Razorbill.Sphinx.Core.Configurations;
    using Fanex.Razorbill.Sphinx.Service.Common.UnderMaintenance;

    /// <summary>
    /// Under maintenance controller.
    /// </summary>
    public class UnderMaintenanceController : Controller
    {        
        /// <summary>
        /// Index page.
        /// </summary>
        /// <returns>Return view.</returns>
        public ActionResult Index()
        {
            var mainternanceInfo = new UnderMaintenanceService().CheckUM();
            if (!mainternanceInfo.IsUM && !WebSettings.IsUMTesting)
            {
                return RedirectToAction("Index", "Login");
            }
            
            ViewBag.UMFrom = mainternanceInfo.From;
            ViewBag.UMTo = mainternanceInfo.To;
            return PartialView("~/Views/Common/UnderMaintenance/Index.cshtml");
        }

        public JsonResult UnhandleException()
        {
            return null;
        }
    }
}