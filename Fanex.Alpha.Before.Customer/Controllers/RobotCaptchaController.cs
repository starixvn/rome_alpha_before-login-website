namespace Fanex.Alpha.Before.Customer.Controllers
{
    using Fanex.Alpha.Before.Common.Configurations;
    using Fanex.Alpha.Before.Common.Controllers;
    using Fanex.Alpha.Before.Common.Helpers;
    using Fanex.Alpha.Before.Customer.Resources;
    using Fanex.Alpha.Before.Customer.Services.SignIn;
    using FPS.Browserless.Detection.Client;
    using global::Alpha.Captcha.Models;
    using global::Alpha.Captcha.Mvc;
    using System;
    using System.Globalization;
    using System.Threading;
    using System.Web.Mvc;
    using System.Web.Security;

    public class RobotCaptchaController : BaseController
    {
        public ActionResult Index(string returnUrl)
        {
            //if (IsNotRobot())
            //{
            //    return Redirect(FormsAuthentication.LoginUrl);
            //}

            if (IsIgnoreRobotCaptcha())
            {
                return Redirect(EncryptURLHelper.BuildAfterURL(HttpContext.ApplicationInstance.Context));
            }

            AppSession.IncreaseRobotCaptchaFailedCount();

            return View("~/Areas/Customer/Views/RobotCaptcha/Index.cshtml", GetCaptchaSetting());
        }

        [HttpPost, CaptchaValidation]
        public ActionResult Index()
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction(nameof(InputCaptchaSuccessfully));
            }

            TempData["ErrorMessage"] = Labels.errInvalidCaptcha;

            return RedirectToAction(nameof(Index));
        }

        public ActionResult InputCaptchaSuccessfully()
            => Redirect(EncryptURLHelper.BuildAfterURL(HttpContext.ApplicationInstance.Context));

        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            var currentThread = Thread.CurrentThread;
            currentThread.CurrentCulture = new CultureInfo(AppSession.Language ?? LanguageHelper.DefaultLanguage);
            currentThread.CurrentUICulture = currentThread.CurrentCulture;

            return base.BeginExecuteCore(callback, state);
        }

        private static bool IsNotRobot()
            => AppSession.RobotResult == null
            || AppSession.RobotResult == BotFlagged.IsClean.ToString();

        private static bool IsIgnoreRobotCaptcha()
            => AppSetting.IsIgnoreRobotCaptcha(AppSession.SuperId, AppSession.Site);

        private static CaptchaSetting GetCaptchaSetting()
            => new CaptchaSetting
            {
                FunctionName = "RobotCaptcha",
                Level = CaptchaLevels.Low
            };
    }
}
