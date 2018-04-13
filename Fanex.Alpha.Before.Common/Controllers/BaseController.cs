namespace Fanex.Alpha.Before.Common.Controllers
{
    using System.Globalization;
    using System.Threading;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Fanex.Alpha.Before.Common.Configurations;
    using Fanex.Alpha.Before.Common.Configurations.UnderMainternance;
    using Fanex.Alpha.Before.Common.Services.LimitedCountryAccessService;
    using Fanex.Alpha.Before.Common.Services.UnderMainternance;

    public class BaseController : Controller
    {
        private readonly IUnderMainternanceService _mainternanceService;
        private readonly ILimitedCountryAccessService _limitedCountryAccessService;

        public BaseController()
            : this(null, null)
        {
        }

        public BaseController(
            UnderMainternanceService mainternanceService,
            LimitedCountryAccessService limitedCountryAccessService)
        {
            _mainternanceService = mainternanceService ?? new UnderMainternanceService();
            _limitedCountryAccessService = limitedCountryAccessService ?? new LimitedCountryAccessService(AppSetting.TrackRunTime);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var underMaintenanceInfo = _mainternanceService.UnderMainternanceInfo();

            if (underMaintenanceInfo.HasError)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "ErrorPage",
                    action = "Index",
                    area = "Common"
                }));
            }
            else if (underMaintenanceInfo.IsUM)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Offline",
                    action = "Index",
                    area = "Common"
                }));
            }
            else if (_limitedCountryAccessService.IsLimitedAccess())
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

        protected override void ExecuteCore()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(AppSession.Language);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            base.ExecuteCore();
        }
    }
}