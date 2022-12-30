namespace Fanex.Alpha.Before.Customer.Controllers
{
    public class RobotCaptchaController
    {
        // private const int LimitRobotCaptchaFailedCount = 5;
        // private readonly ICaptchaService captchaService;
        // private readonly CaptchaOptions captchaOptions;

        // public RobotCaptchaController()
        //     : this(null, null)
        // {
        // }

        // public RobotCaptchaController(CaptchaOptions captchaOptions, IServiceFactory serviceFactory)
        // {
        //     this.captchaOptions = captchaOptions ?? SliderCaptchaHandler.CaptchaOptions;
        //     serviceFactory = serviceFactory ?? SliderCaptchaHandler.ServiceFactory;
        //     captchaService = serviceFactory.CaptchaService;
        // }

        // private static string TooManyFailedLoginPageUrl => $"{FormsAuthentication.LoginUrl}?r={Labels.tooManyFailedCaptcha}";

        // public Microsoft.AspNetCore.Mvc.ActionResult Index(string returnUrl)
        // {
        //     if (IsNotRobot())
        //     {
        //         var message = $"{Labels.invalidsession} / {Labels.inputCaptchaTooLong}";

        //         return Redirect($"{FormsAuthentication.LoginUrl}?r={message}");
        //     }

        //     if (IsIgnoreRobotCaptcha())
        //     {
        //         return Redirect(EncryptURLHelper.BuildAfterURL(HttpContext.ApplicationInstance.Context));
        //     }

        //     AssignDarkModeNameViewData();

        //     ViewBag.tooManyFailedLoginPageUrl = TooManyFailedLoginPageUrl;
        //     return View("~/Areas/Customer/Views/RobotCaptcha/Index.cshtml");
        // }

        // [HttpPost]
        // public Microsoft.AspNetCore.Mvc.ActionResult Index(RobotCaptchaModel model)
        // {
        //     var isVerifiedResult = captchaService.IsVerfied(model.Token);
        //     if (isVerifiedResult.IsSuccess)
        //     {
        //         AppSession.InputCaptchaSuccess = true;
        //         return RedirectToAction(nameof(InputCaptchaSuccessfully));
        //     }

        //     if (captchaOptions.LogEnabled)
        //     {
        //         var exception = new Exception($"robot captcha: key={AppSession.Username}, code={isVerifiedResult.Code}, reason={isVerifiedResult.Reason}, detail={isVerifiedResult.Detail}");
        //         ExceptionHandler.Handle(exception, TraceEventType.Warning);
        //     }

        //     if (AppSession.CustId == 0)
        //     {
        //         return Redirect($"{FormsAuthentication.LoginUrl}?r={Labels.inputCaptchaTooLong}");
        //     }

        //     TempData["ErrorMessage"] = Labels.errInvalidCaptcha;

        //     AppSession.IncreaseRobotCaptchaFailedCount();

        //     if (AppSession.RobotCaptchaFailedCount >= LimitRobotCaptchaFailedCount)
        //     {
        //         TempData["ErrorMessage"] = string.Empty;

        //         return Redirect(TooManyFailedLoginPageUrl);
        //     }

        //     return RedirectToAction(nameof(Index));
        // }

        // public Microsoft.AspNetCore.Mvc.ActionResult InputCaptchaSuccessfully()
        // {
        //     if (AppSession.InputCaptchaSuccess)
        //     {
        //         return Redirect(EncryptURLHelper.BuildAfterURL(HttpContext.ApplicationInstance.Context));
        //     }

        //     return RedirectToAction(nameof(Index));
        // }

        // protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        // {
        //     var currentThread = Thread.CurrentThread;
        //     currentThread.CurrentCulture = new CultureInfo(AppSession.Language ?? LanguageHelper.DefaultLanguage);
        //     currentThread.CurrentUICulture = currentThread.CurrentCulture;

        //     return base.BeginExecuteCore(callback, state);
        // }

        // private static bool IsNotRobot()
        //     => string.IsNullOrWhiteSpace(AppSession.RobotResult)
        //     || AppSession.RobotResult == ((int)BotFlagged.IsClean).ToString();

        // private static bool IsIgnoreRobotCaptcha()
        //     => AppSetting.IsIgnoreRobotCaptcha(AppSession.SuperId, AppSession.Site);
        // public RobotCaptchaController(IUnderMainternanceService mainternanceService, ILimitedCountryAccessService limitedCountryAccessService)
        //     : base(mainternanceService, limitedCountryAccessService)
        // {
        // }
    }
}
