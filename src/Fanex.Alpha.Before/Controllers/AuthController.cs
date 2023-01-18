using Fanex.Alpha.Before.Constants;
using Fanex.Alpha.Before.Helpers;
using Fanex.Alpha.Before.Models;
using Fanex.Alpha.Before.Services.SignIn;
using Microsoft.AspNetCore.Mvc;
using ONEbook.UM.AspNetCore.Filters;

namespace Fanex.Alpha.Before.Controllers;

[ServiceFilter(typeof(CheckUMFilterAttribute))]
public class AuthController : Controller
{
    private readonly ISignInService signInService;

    public AuthController(ISignInService signInService)
    {
        this.signInService = signInService;
    }

    [HttpGet]
    public IActionResult Index(string r = null)
    {
        var umToken = Request.Query["UMToken"].ToString();
        if (!string.IsNullOrWhiteSpace(umToken))
        {
            HttpContext.Session.SetString("UMToken", umToken);
        }

        var loginResult = signInService.SignIn(HttpContext);
        loginResult.ErrCode = !string.IsNullOrWhiteSpace(r) ? SignInErrorContants.ServerError : loginResult.ErrCode;
        loginResult.ErrMsg = !string.IsNullOrWhiteSpace(r) ? r : loginResult.ErrMsg;

        ViewBag.FPSToken = FPS.Client.NetStandard.Token.Generate();
        AppSession.ResetRobotCaptchaSession();
        AssignDarkModeNameViewData();

        return View(loginResult);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(
        string txtUserName,
        string txtPassWord,
        string hidLanguage,
        string code,
        string captchaToken,
        string browserSize,
        string deviceId = "N/A",
        string assetAppPath = "")
    {
        AssignSettingLanguage(hidLanguage);
        ViewBag.FPSToken = FPS.Client.NetStandard.Token.Generate();
        var model = new SignInModel(txtUserName, txtPassWord, hidLanguage, code, captchaToken, browserSize, deviceId, assetAppPath);
        var loginResult = await signInService.SignIn(model, HttpContext);
        if (loginResult.ErrCode == SignInErrorContants.Success && !string.IsNullOrEmpty(loginResult.AfterLoginUrl))
        {
            if (loginResult.AfterLoginUrl.IndexOf("RobotCaptcha", StringComparison.OrdinalIgnoreCase) != -1)
            {
                FPSHelper.FPSTracking(loginResult);
            }

            return Redirect(loginResult.AfterLoginUrl);
        }

        AssignDarkModeNameViewData();

        return View(loginResult);
    }

    public IActionResult AutomationTest(string username, string password, string captcha, string deviceId, string browserSize)
    {
        var loginResult = signInService.AutomationTest(username, password, captcha, deviceId, browserSize, HttpContext);

        if (loginResult.ErrCode == -1)
        {
            return Redirect(loginResult.AfterLoginUrl);
        }

        return Index();
    }

    private static void AssignSettingLanguage(string language)
    {
        LanguageHelper.SetLanguage(LanguageHelper.IsValidLanguage(language) ? language : LanguageHelper.DefaultLanguage);
    }

    private void AssignDarkModeNameViewData()
        => ViewData["DarkModeName"] = AppSession.DarkModeName;
}