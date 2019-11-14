namespace Fanex.Alpha.Before.Common.Controllers
{
    using System.Web.Mvc;
    using Configurations;
    using Services.LimitedCountryAccessService;

    public class LimitedCountryAccessController : Controller
    {
        private readonly ILimitedCountryAccessService limitedCountryAccessService;

        public LimitedCountryAccessController()
            : this(null)
        {
        }

        public LimitedCountryAccessController(ILimitedCountryAccessService limitedCountryAccessService)
        {
            this.limitedCountryAccessService = limitedCountryAccessService ?? new LimitedCountryAccessService(AppSetting.TrackRunTime);
        }

        public ActionResult Index()
        {
            if (limitedCountryAccessService.IsLimitedAccess())
            {
                return IndexView();
            }
            else
            {
                return Redirect(Helpers.UrlHelper.SiteGroupSetting.BeforeUrl);
            }
        }

        public ActionResult IndexView() => View("~/Views/Common/LimitedCountryAccess/Index.cshtml");
    }
}