// -----------------------------------------------------------------------
// <copyright file="BaseController.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Alpha.Before.Common.Controllers
{
    using System.Globalization;
    using System.Threading;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Fanex.Alpha.Before.Common.Configurations;
    using Fanex.Alpha.Before.Common.Configurations.LimitedCountryAccessService;
    using Fanex.Alpha.Before.Common.Configurations.UnderMainternance;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class BaseController : Controller
    {
        private IUnderMainternanceService _mainternanceService;
        private ILimitedCountryAccessService _limitedCountryAccessService;

        public BaseController()
            : this(null, null)
        {
        }

        public BaseController(
            UnderMainternanceService mainternanceService,
            LimitedCountryAccessService limitedCountryAccessService)
        {
            _mainternanceService = _mainternanceService ?? new UnderMainternanceService();
            _limitedCountryAccessService = limitedCountryAccessService ?? new LimitedCountryAccessService();
        }

        /// <summary>
        /// Execute before each request, if Downtime then go to Downtime page
        /// Or not Authenticate will go to Login Page.
        /// </summary>
        /// <param name="filterContext">Filter context.</param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (_mainternanceService.UnderMainternanceInfo().IsUM)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Offline",
                    action = "Index",
                    area = "Common"
                }));
            }
            else if (_limitedCountryAccessService.LimitedAccess())
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "LimitedCountryAccess",
                    action = "Index",
                    area = "Common"
                }));
            }

            base.OnActionExecuting(filterContext);
        }

        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
        }

        protected override void ExecuteCore()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(AppSession.Language);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            base.ExecuteCore();
        }
    }
}
