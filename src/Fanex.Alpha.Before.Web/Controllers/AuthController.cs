using System.Net;
using System.Text;
using Fanex.Alpha.Before.Common.Configurations;
using Fanex.Alpha.Before.Common.Helpers;
using Fanex.Alpha.Before.Customer.Constants;
using Fanex.Alpha.Before.Customer.Helpers;
using Fanex.Alpha.Before.Customer.Models;
using Fanex.Alpha.Before.Customer.Services.SignIn;
using Microsoft.AspNetCore.Mvc;
using ONEbook.UM.AspNetCore.Filters;

namespace Fanex.Alpha.Before.Web.Controllers;

[ServiceFilter(typeof(CheckUMFilterAttribute))]
public class AuthController : Controller
{
    private readonly ISignInService _signInService;

    public AuthController(ISignInService signInService)
    {
        _signInService = signInService;
    }

    [HttpGet]
    public ActionResult Index(string r = null)
    {
        var umToken = Request.Query["UMToken"].ToString();
        if (!string.IsNullOrWhiteSpace(umToken))
        {
            HttpContext.Session.SetString("UMToken", umToken);
        }

        var loginResult = _signInService.SignIn(HttpContext);
        loginResult.ErrCode = !string.IsNullOrWhiteSpace(r) ? SignInErrorContants.ServerError : loginResult.ErrCode;
        loginResult.ErrMsg = !string.IsNullOrWhiteSpace(r) ? r : loginResult.ErrMsg;

        //ViewBag.FPSToken = FPS.Client.Token.Generate();
        AppSession.ResetRobotCaptchaSession();
        AssignDarkModeNameViewData();

        return View(loginResult);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public ActionResult Index(
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
        //ViewBag.FPSToken = FPS.Client.Token.Generate();
        var model = new SignInModel(txtUserName, txtPassWord, hidLanguage, code, captchaToken, browserSize, deviceId, assetAppPath);
        var loginResult = _signInService.SignIn(model, HttpContext);
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

    [HttpPost]
    public async Task<ActionResult> ValidateGoogleCaptcha(string token)
    {
        var googleReCaptcha = UrlHelper.SiteGroupSetting.GoogleReCaptcha;

        try
        {
            var TARGETURL = string.Format(
               "https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}",
               googleReCaptcha.SecretKey,
               token);

            var handler = new HttpClientHandler
            {
                Proxy = new WebProxy(googleReCaptcha.Proxy),
                UseProxy = true,
            };

            var client = new HttpClient(handler);
            var byteArray = Encoding.ASCII.GetBytes(":");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            var response = await client.GetAsync(TARGETURL);
            var content = response.Content;

            var result = await content.ReadAsStringAsync();

            return Json(result);
        }
        catch
        {
            return Json(string.Empty);
        }
    }

    public ActionResult AutomationTest(string username, string password, string captcha, string deviceId, string browserSize)
    {
        var loginResult = _signInService.AutomationTest(username, password, captcha, deviceId, browserSize, HttpContext);

        if (loginResult.ErrCode == -1)
        {
            return Redirect(loginResult.AfterLoginUrl);
        }

        return Index();
    }

    /// <summary>
    /// Setting language that user's choosing.
    /// </summary>
    /// <param name="language">The currently language.</param>
    private static void AssignSettingLanguage(string language)
    {
        LanguageHelper.SetLanguage(LanguageHelper.IsValidLanguage(language) ? language : LanguageHelper.DefaultLanguage);
    }

    protected void AssignDarkModeNameViewData()
        => ViewData["DarkModeName"] = AppSession.DarkModeName;
}