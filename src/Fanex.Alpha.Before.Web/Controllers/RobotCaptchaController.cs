using Alpha.SliderCaptcha.Core;
using Alpha.SliderCaptcha.Core.Services;
using Fanex.Alpha.Before.Configurations;
using Fanex.Alpha.Before.Constants;
using Fanex.Alpha.Before.Models;
using Fanex.Alpha.Before.Resources;
using Fanex.Alpha.Before.Services.SignIn;
using Fanex.Logging.Extensions.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Alpha.Before.Web.Controllers;

public class RobotCaptchaController : Controller
{
    private const int LimitRobotCaptchaFailedCount = 5;
    private readonly ICaptchaService captchaService;
    private readonly CaptchaOptions captchaOptions;

    public RobotCaptchaController(CaptchaOptions captchaOptions, ICaptchaService captchaService)
    {
        this.captchaOptions = captchaOptions;
        this.captchaService = captchaService;
    }

    private static string TooManyFailedLoginPageUrl => $"~/?r={Labels.tooManyFailedCaptcha}";

    public IActionResult Index()
    {
        if (IsNotRobot())
        {
            var message = $"{Labels.invalidsession} / {Labels.inputCaptchaTooLong}";

            return Redirect($"~/?r={message}");
        }

        if (IsIgnoreRobotCaptcha())
        {
            return Redirect(EncryptUrlHelper.BuildAfterURL(HttpContext));
        }

        AssignDarkModeNameViewData();

        ViewBag.tooManyFailedLoginPageUrl = TooManyFailedLoginPageUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(RobotCaptchaModel model)
    {
        var isVerifiedResult = captchaService.IsVerfied(model.Token);
        if (isVerifiedResult.IsSuccess)
        {
            AppSession.InputCaptchaSuccess = true;
            return RedirectToAction(nameof(InputCaptchaSuccessfully));
        }

        if (captchaOptions.LogEnabled)
        {
            var exception = new Exception($"robot captcha: key={AppSession.Username}, code={isVerifiedResult.Code}, reason={isVerifiedResult.Reason}, detail={isVerifiedResult.Detail}");
            await ExceptionHandler.HandleAsync(exception, HttpContext);
        }

        if (AppSession.CustId == 0)
        {
            return Redirect($"~/?r={Labels.inputCaptchaTooLong}");
        }

        TempData["ErrorMessage"] = Labels.errInvalidCaptcha;

        AppSession.IncreaseRobotCaptchaFailedCount();

        if (AppSession.RobotCaptchaFailedCount >= LimitRobotCaptchaFailedCount)
        {
            TempData["ErrorMessage"] = string.Empty;

            return Redirect(TooManyFailedLoginPageUrl);
        }

        return RedirectToAction(nameof(Index));
    }

    public IActionResult InputCaptchaSuccessfully()
    {
        if (AppSession.InputCaptchaSuccess)
        {
            return Redirect(EncryptUrlHelper.BuildAfterURL(HttpContext));
        }

        return RedirectToAction(nameof(Index));
    }

    private static bool IsNotRobot()
        => string.IsNullOrWhiteSpace(AppSession.RobotResult)
        || AppSession.RobotResult == ((int)BotFlagged.IsClean).ToString();

    private static bool IsIgnoreRobotCaptcha()
        => AppSetting.IsIgnoreRobotCaptcha(AppSession.SuperId, AppSession.Site);

    private void AssignDarkModeNameViewData()
        => ViewData["DarkModeName"] = AppSession.DarkModeName;
}