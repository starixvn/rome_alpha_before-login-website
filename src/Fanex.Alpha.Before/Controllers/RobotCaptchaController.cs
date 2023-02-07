using Alpha.Captcha.Core;
using Alpha.Captcha.Core.Services;
using Fanex.Alpha.Before.Configurations;
using Fanex.Alpha.Before.Models;
using Fanex.Alpha.Before.Resources;
using Fanex.Alpha.Before.Services.SignIn;
using Fanex.Logging.Extensions.AspNetCore;
using FPS.Browserless.Detection.Client.NetStandard.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Fanex.Alpha.Before.Controllers;

public class RobotCaptchaController : Controller
{
    private const int LimitRobotCaptchaFailedCount = 5;
    private readonly ICaptchaService captchaService;
    private readonly CaptchaOptions captchaOptions;
    private readonly RobotOptions robotOptions;
    private readonly IEncryptUrlHelper encryptUrlHelper;

    public RobotCaptchaController(
        CaptchaOptions captchaOptions,
        ICaptchaService captchaService,
        IOptions<RobotOptions> robotOptions,
        IEncryptUrlHelper encryptUrlHelper)
    {
        this.captchaOptions = captchaOptions;
        this.captchaService = captchaService;
        this.robotOptions = robotOptions.Value;
        this.encryptUrlHelper = encryptUrlHelper;
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
            return Redirect(encryptUrlHelper.BuildAfterURL(HttpContext));
        }

        AssignDarkModeNameViewData();

        ViewBag.tooManyFailedLoginPageUrl = TooManyFailedLoginPageUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(RobotCaptchaModel model)
    {
        var isVerifiedResult = await captchaService.IsVerified(model.Token);
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
            return Redirect(encryptUrlHelper.BuildAfterURL(HttpContext));
        }

        return RedirectToAction(nameof(Index));
    }

    private static bool IsNotRobot()
        => string.IsNullOrWhiteSpace(AppSession.RobotResult)
        || AppSession.RobotResult == ((int)BotFlagged.IsClean).ToString();

    private bool IsIgnoreRobotCaptcha()
        => robotOptions.ExcludeSuperIds.Contains(AppSession.SuperId)
        || robotOptions.ExcludeSites.Contains(AppSession.Site.ToUpperInvariant());

    private void AssignDarkModeNameViewData()
        => ViewData["DarkModeName"] = AppSession.DarkModeName;
}