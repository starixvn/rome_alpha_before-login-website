using System.Web.Routing;
using Fanex.Razorbill.Sphinx.Service.Common.UnderMaintenance;

namespace Fanex.Razorbill.Sphinx.Web.Controllers
{
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Mvc;

    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public class BaseController : Controller
    {

        
        public BaseController()
        {
        }

        /// <summary>
        /// Execute before each request, if Downtime then go to Downtime page
        /// Or not Authenticate will go to Login Page
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var undermaintenanceService = new UnderMaintenanceService();
            var umInfo = undermaintenanceService.CheckUM();
            if (umInfo.IsUM)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "UnderMaintenance",
                    action = "Index"
                }));


            }
            base.OnActionExecuting(filterContext);
        }

        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {            
        }

        
    }
}