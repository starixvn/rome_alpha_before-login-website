namespace Fanex.Alpha.Before.Common.Controllers
{
    using System.Web.Mvc;
    using System.Web.Security;
    using Configurations.UnderMainternance;
    using Services.UnderMainternance;

    public class OfflineController : Controller
    {
        private readonly IUnderMainternanceService _mainternanceService;

        public OfflineController() :
            this(null)
        {
        }

        public OfflineController(IUnderMainternanceService mainternanceService)
        {
            _mainternanceService = mainternanceService ?? new UnderMainternanceService();
        }

        /// <summary>
        /// Checking down time.
        /// </summary>
        /// <returns>Return result.</returns>
        [HttpGet]
        public ActionResult Index()
        {
            var downTime = _mainternanceService.UnderMainternanceInfo();
            if (!downTime.IsUM)
            {
                if (Request.QueryString["UMToken"] != null)
                {
                    Session["UMToken"] = Request.QueryString["UMToken"];
                }

                FormsAuthentication.RedirectToLoginPage();
            }

            return View("~/Views/Common/UnderMaintenance/Index.cshtml", downTime);
        }
    }
}