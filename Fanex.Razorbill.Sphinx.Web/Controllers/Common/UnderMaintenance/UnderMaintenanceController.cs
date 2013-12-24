
namespace Fanex.Razorbill.Sphinx.Web.Controllers.Common.UnderMaintenance
{
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Mvc;
    using Fanex.Razorbill.Sphinx.Service.Common.UnderMaintenance;
    using System.Web.Security;

    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public class UnderMaintenanceController : Controller
    {        
        public ActionResult Index()
        {
            var underMaintenanceService = new UnderMaintenanceService();
            var umInfo = underMaintenanceService.CheckUM();
            if (!umInfo.IsUM)
            {
                return RedirectToAction("Index", "Login");
            }
            
            ViewBag.UMFrom = umInfo.From;
            ViewBag.UMTo = umInfo.To;
            return PartialView("~/Views/Common/UnderMaintenance/Index.cshtml");
        }

        public JsonResult UnhandleException()
        {
            return null;
        }
    }
}