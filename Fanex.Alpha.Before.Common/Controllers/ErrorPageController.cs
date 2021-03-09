namespace Fanex.Alpha.Before.Common.Controllers
{
    using System.Web.Mvc;

    public class ErrorPageController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.ErrorMessage = Request != null ? Request.RequestContext.HttpContext.IsCustomErrorEnabled ?
                Infrastructure.Resources.Labels.Error_Server : Server.GetLastError().ToString() : string.Empty;
            return View("~/Views/Common/ErrorPage/Index.cshtml");
        }
    }
}