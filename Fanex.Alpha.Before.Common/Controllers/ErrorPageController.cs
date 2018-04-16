namespace Fanex.Alpha.Before.Common.Controllers
{
    using System.Web.Mvc;

    public class ErrorPageController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.ErrorMessage = Infrastructure.Resources.Labels.Error_Server;
            return View("~/Views/Common/ErrorPage/Index.cshtml");
        }
    }
}