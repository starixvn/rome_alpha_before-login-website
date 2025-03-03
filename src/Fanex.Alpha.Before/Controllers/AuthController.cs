using System.Text.Json;
using Fanex.Alpha.Before.Configurations;
using Fanex.Alpha.Before.Constants;
using Fanex.Alpha.Before.Helpers;
using Fanex.Alpha.Before.Models;
using Fanex.Alpha.Before.Resources;
using Fanex.Alpha.Before.Services;
using Fanex.Alpha.Before.Services.SignIn;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ONEbook.UM.AspNetCore.Filters;

namespace Fanex.Alpha.Before.Controllers;

[ServiceFilter(typeof(CheckUMFilterAttribute))]
public class AuthController : Controller
{
    private readonly IProfilePictureService profilePictureService;
    private readonly ISignInService signInService;
    private readonly IFPSHelper fpsHelper;
    private readonly IDistributedCacheService cacheService;
    private readonly CommonOptions commonOptions;
    private readonly IPHelper ipHelper;
    private readonly Logging.ILogger logger;

    public AuthController(
        IProfilePictureService profilePictureService,
        ISignInService signInService,
        IFPSHelper fpsHelper,
        IDistributedCacheService cacheService,
        IOptions<CommonOptions> commonOptions,
        IPHelper ipHelper,
        Logging.ILogger logger)
    {
        this.profilePictureService = profilePictureService;
        this.signInService = signInService;
        this.fpsHelper = fpsHelper;
        this.cacheService = cacheService;
        this.commonOptions = commonOptions.Value;
        this.ipHelper = ipHelper;
        this.logger = logger;
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
        string assetAppPath = "",
        string dcsStaticAppPath = "")
    {
        AssignSettingLanguage(hidLanguage);
        ViewBag.FPSToken = FPS.Client.NetStandard.Token.Generate();
        var model = new SignInModel(
            txtUserName,
            txtPassWord,
            hidLanguage,
            code,
            captchaToken,
            browserSize,
            fpsHelper.GetDeviceID(HttpContext.Request),
            assetAppPath,
            dcsStaticAppPath);
        var loginResult = await signInService.SignIn(model, HttpContext);


        if (loginResult.ErrCode == SignInErrorContants.Success && !string.IsNullOrEmpty(loginResult.AfterLoginUrl))
        {
            fpsHelper.FPSTracking(HttpContext.Request, new Models.SignInResult
            {
                Username = loginResult.Username.Trim(),
                LoginTime = loginResult.LoginTime
            });

            return Redirect(loginResult.AfterLoginUrl);
        }

        AssignDarkModeNameViewData();
        loginResult.IsVerified = true;
        var custId = await signInService.CheckUsername(loginResult.Username, HttpContext);
        var profilePicture = await profilePictureService.GetProfilePicture(custId);
        loginResult.Avatar = profilePicture.ImageBase64String;

        return View(loginResult);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Verify(string username)
    {
        var host = Request.Host.Value.TrimEnd('.');
        if (!AppSetting.DomainMappings.ContainsKey(host))
        {
            return BadRequest();
        }

        if (await cacheService.IsLoginIPBlocked())
        {
            var ipLockedError = string.Format(
                   Labels.iploginfailedmultipletimeserror,
                   ipHelper.ClientIP(),
                   commonOptions.MaxTimesToAllowLoginFail,
                   commonOptions.BlockLoginMinutes);

            return Json(VerifyResult.CreateErrorResult(ipLockedError, SignInErrorContants.IPBlocked));
        }

        var custId = await signInService.CheckUsername(username, HttpContext);
        if (custId <= 0)
        {
            var loginIPFailedCount = await cacheService.IncreaseLoginIPFailedCount();
            if (loginIPFailedCount > commonOptions.MaxTimesToAllowLoginFail)
            {
                if (loginIPFailedCount == commonOptions.MaxTimesToAllowLoginFail + 1)
                {
                    await logger.WarnAsync($"IP blocked for login: {JsonSerializer.Serialize(DeviceHelper.GetRequestInfo(HttpContext.Request, string.Empty, ipHelper.ClientIP()))}");
                }

                await cacheService.SetLoginIPBlocked(commonOptions.BlockLoginMinutes);
                var ipLockedError = string.Format(
                    Labels.iploginfailedmultipletimeserror,
                    ipHelper.ClientIP(),
                    commonOptions.MaxTimesToAllowLoginFail,
                    commonOptions.BlockLoginMinutes);
                return Json(VerifyResult.CreateErrorResult(ipLockedError, SignInErrorContants.IPBlocked));
            }
        }
        var profilePicture = await profilePictureService.GetProfilePicture(custId);
        return Ok(VerifyResult.CreateSuccessResult(profilePicture.ImageBase64String));
    }

    public async Task<IActionResult> AutomationTest(string username, string password, string captcha, string deviceId, string browserSize)
    {
        var loginResult = await signInService.AutomationTest(username, password, captcha, deviceId, browserSize, HttpContext);

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