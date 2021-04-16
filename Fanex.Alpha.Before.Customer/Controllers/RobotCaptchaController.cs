namespace Fanex.Alpha.Before.Customer.Controllers
{
    using System;
    using System.Globalization;
    using System.Threading;
    using System.Web.Mvc;
    using System.Web.Security;
    using Fanex.Alpha.Before.Common.Configurations;
    using Fanex.Alpha.Before.Common.Controllers;
    using Fanex.Alpha.Before.Common.Helpers;
    using Fanex.Alpha.Before.Customer.Models;
    using Fanex.Alpha.Before.Customer.Resources;
    using Fanex.Alpha.Before.Customer.Services.SignIn;
    using FPS.Browserless.Detection.Client;
    using global::Alpha.SliderCaptcha.Mvc;
    using global::Alpha.SliderCaptcha.Services;

    public class RobotCaptchaController : BaseController
    {
        private readonly ICaptchaService captchaService;

        public RobotCaptchaController()
            : this(null)
        {
        }

        public RobotCaptchaController(IServiceFactory serviceFactory)
        {
            serviceFactory = serviceFactory ?? Configuration.ServiceFactory;
            captchaService = serviceFactory.CaptchaService;
        }

        public ActionResult Index(string returnUrl)
        {
            if (IsNotRobot())
            {
                var message = $"{Labels.invalidsession} / {Labels.inputCaptchaTooLong}";

                return Redirect($"{FormsAuthentication.LoginUrl}?r={message}");
            }

            if (IsIgnoreRobotCaptcha())
            {
                return Redirect(EncryptURLHelper.BuildAfterURL(HttpContext.ApplicationInstance.Context));
            }

            return View("~/Areas/Customer/Views/RobotCaptcha/Index.cshtml");
        }

        [HttpPost]
        public ActionResult Index(RobotCaptchaModel model)
        {
            if (captchaService.IsVerfied(model.Token))
            {
                AppSession.InputCaptchaSuccess = true;
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
        {
            if (AppSession.InputCaptchaSuccess)
            {
                return Redirect(EncryptURLHelper.BuildAfterURL(HttpContext.ApplicationInstance.Context));
            }

            return RedirectToAction(nameof(Index));
        }

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
    }
}
