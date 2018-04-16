namespace Fanex.Alpha.Before.Common.Controllers
{
    using System.Web.Mvc;

    public class LimitedCountryAccessController : Controller
    {
        public ActionResult Index()
        {
            return View("~/Views/Common/LimitedCountryAccess/Index.cshtml");
        }
    }
}
