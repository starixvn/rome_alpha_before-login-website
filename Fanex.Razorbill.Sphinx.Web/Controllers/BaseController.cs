// -----------------------------------------------------------------------
// <copyright file="BaseController.cs" company="Nexcel Solution">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Razorbill.Sphinx.Web.Controllers
{
    using System.Web.Mvc;
    using System.Web.Routing;
    using Fanex.Razorbill.Sphinx.Service.Common.UnderMaintenance;

    /// <summary>
    /// Base controller.
    /// </summary>
    public class BaseController : Controller
    {
        public BaseController()
        {
        }

        /// <summary>
        /// Execute before each request, if Downtime then go to Downtime page
        /// Or not Authenticate will go to Login Page.
        /// </summary>
        /// <param name="filterContext">Filter context.</param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (new UnderMaintenanceService().CheckUM().IsUM)
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