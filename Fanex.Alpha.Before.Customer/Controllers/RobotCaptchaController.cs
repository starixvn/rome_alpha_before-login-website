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
            if (IsNotRobot())
            {
                var message = Request.UrlReferrer == null ||
                    Request.UrlReferrer.ToString().IndexOf(Request.Headers["Host"], StringComparison.OrdinalIgnoreCase) == -1 ?
                    string.Empty : Labels.inputCaptchaTooLong;

                return Redirect($"{FormsAuthentication.LoginUrl}?r={message}");
            }

            if (IsIgnoreRobotCaptcha())
            {
                return Redirect(EncryptURLHelper.BuildAfterURL(HttpContext.ApplicationInstance.Context));
            }

            return View("~/Areas/Customer/Views/RobotCaptcha/Index.cshtml", GetCaptchaSetting());
        }

        [HttpPost, CaptchaValidation]
        public ActionResult Index()
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction(nameof(InputCaptchaSuccessfully));
            }

            if (AppSession.CustId == 0)
            {
                return Redirect($"{FormsAuthentication.LoginUrl}?r={Labels.inputCaptchaTooLong}");
            }

            TempData["ErrorMessage"] = Labels.errInvalidCaptcha;

            AppSession.IncreaseRobotCaptchaFailedCount();

            if (AppSession.RobotCaptchaFailedCount >= 5)
            {
                TempData["ErrorMessage"] = string.Empty;

                return Redirect($"{FormsAuthentication.LoginUrl}?r={Labels.tooManyFailedCaptcha}");
            }

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
            => string.IsNullOrWhiteSpace(AppSession.RobotResult)
            || AppSession.RobotResult == ((int)BotFlagged.IsClean).ToString();

        private static bool IsIgnoreRobotCaptcha()
            => AppSetting.IsIgnoreRobotCaptcha(AppSession.SuperId, AppSession.Site);

        private static CaptchaSetting GetCaptchaSetting()
            => new CaptchaSetting
            {
                FunctionName = "RobotCaptcha",
                CaptchaLevel = CaptchaLevels.Low
            };
    }
}
