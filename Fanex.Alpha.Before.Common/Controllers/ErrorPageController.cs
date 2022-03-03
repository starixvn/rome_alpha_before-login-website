namespace Fanex.Alpha.Before.Common.Controllers
{
    using System.Data;
    using System.Data.Common;
    using System.Web;
    using System.Web.Mvc;
    using Fanex.Alpha.Before.Common.Configurations;
    using Fanex.Alpha.Before.Common.Helpers;

    public class ErrorPageController : Controller
    {
        public ActionResult Index()
        {
            var ex = Server?.GetLastError();
            var inner = ex?.InnerException;
            var errCode = Request?["code"];

            if (string.IsNullOrWhiteSpace(errCode))
            {
                if (ex is DbException || ex is EntityException || inner is DbException || inner is EntityException)
                {
                    errCode = "5002";
                }
                else if (ex is HttpException)
                {
                    errCode = ((HttpException)ex).GetHttpCode().ToString();
                }
                else if (inner is HttpException)
                {
                    errCode = ((HttpException)inner).GetHttpCode().ToString();
                }
                else
                {
                    errCode = "5000";
                }
            }

            ViewBag.ErrorMessage = Request != null ? Request.RequestContext.HttpContext.IsCustomErrorEnabled ?
                string.Format(Infrastructure.Resources.Labels.Error_Server, errCode) : Server?.GetLastError()?.ToString() : string.Empty;

            Server?.ClearError();
            ViewData["DarkModeName"] = AppSession.DarkModeName;


            return View("~/Views/Common/ErrorPage/Index.cshtml");
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var language = Request.Cookies["hidLanguage"]?.Value;
            var culture = LanguageHelper.IsValidLanguage(language)
                ? language : LanguageHelper.DefaultLanguage;

            LanguageHelper.SetLanguage(culture);

            base.OnActionExecuting(filterContext);
        }
    }
}