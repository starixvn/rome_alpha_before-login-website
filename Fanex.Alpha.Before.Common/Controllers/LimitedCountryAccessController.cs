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
                return View("~/Views/Common/LimitedCountryAccess/Index.cshtml");
            }
            else
            {
                return RedirectToAction("Index", "SignIn", new { area = "Customer" });
            }
        }
    }
}