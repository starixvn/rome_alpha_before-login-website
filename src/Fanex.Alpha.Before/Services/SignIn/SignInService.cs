using Fanex.Cryptography.Aes;
using Fanex.Alpha.Before.Configurations;
using Fanex.Alpha.Before.Constants;
using Fanex.Alpha.Before.Models;
using Fanex.Alpha.Before.Repositories;
using Fanex.Alpha.Before.Resources;
using Fanex.Alpha.Before.Services.FPS;
using Newtonsoft.Json;
using TelegramBot.Client.Services;
using Fanex.Alpha.Before.Helpers;
using Fanex.Alpha.Before.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Alpha.Captcha.Core;
using Alpha.Captcha.Core.Services;
using Fanex.Alpha.Before.NetThrottling.Models;
using Fanex.Alpha.Before.NetThrottling;

namespace Fanex.Alpha.Before.Services.SignIn
{
    public interface ISignInService
    {
        SignInResult SignIn(HttpContext httpContext);

        Task<SignInResult> SignIn(SignInModel model, HttpContext httpContext);

        SignInResult AutomationTest(string username, string password, string captcha, string deviceId, string browserSize, HttpContext httpContext);
    }

    public class SignInService : ISignInService
    {
        private readonly string _loginTheme;

        private readonly CommonOptions commonOptions;
        private readonly AutomationTestOptions automationTestOptions;
        private readonly IPHelper ipHelper;
        private readonly IPService ipService;
        private readonly ISignInRepository signInRepository;
        private readonly IDistributedCacheService cacheService;
        private readonly IRobotDetector robotDetector;
        private readonly CaptchaOptions captchaOptions;
        private readonly ICaptchaService captchaService;
        private readonly IKickoutCustomerService kickoutCustomerService;
        private readonly INotificationService notificationService;
        private readonly Logging.ILogger logger;
        private readonly ThrottlePolicy throttlePolicy;
        private readonly INetThrottler netThrottler;

        public SignInService(
            IOptions<CommonOptions> commonOptions,
            IOptions<AutomationTestOptions> automationTestOptions,
            IOptions<LoginThemeOptions> loginThemeOptions,
            Logging.ILogger logger,
            IPHelper ipHelper,
            ISignInRepository signInRepository,
            IRobotDetector robotDetector,
            IDistributedCacheService cacheService,
            CaptchaOptions captchaOptions,
            ICaptchaService captchaService,
            IKickoutCustomerService kickoutCustomerService,
            IPService ipService,
            INotificationService notificationService,
            IOptions<ThrottlePolicy> throttlePolicy,
            INetThrottler netThrottler)
        {
            this.commonOptions = commonOptions.Value;
            this.automationTestOptions = automationTestOptions.Value;
            this.logger = logger;
            this.ipHelper = ipHelper;
            this.ipService = ipService;
            this.signInRepository = signInRepository;
            this.cacheService = cacheService;
            this.robotDetector = robotDetector;
            this.captchaOptions = captchaOptions;
            this.captchaService = captchaService;
            this.kickoutCustomerService = kickoutCustomerService;
            this.notificationService = notificationService;
            this.throttlePolicy = throttlePolicy.Value;
            this.netThrottler = netThrottler;

            var loginThemeOpt = loginThemeOptions.Value;
            _loginTheme = loginThemeOpt.DefaultTheme;
            if (DateTime.Now >= loginThemeOpt.StartDate &&
                DateTime.Now <= loginThemeOpt.EndDate)
            {
                _loginTheme = loginThemeOpt.EventTheme;
            }
        }

        public SignInResult SignIn(HttpContext httpContext)
        {
            var loginResult = new SignInResult
            {
                LoginTemplate = _loginTheme,
                Ip = ipHelper.ClientIP(),
                Username = string.Empty,
                NoInteractionTimeInMinutes = commonOptions.NoInteractionTimeInMinutes
            };

            //// Check cross cookie compatibility
            const int EncryptCookieCodeLength = 6;
            AppSession.CookieCode = EncryptExtension.RandomNumberString(EncryptCookieCodeLength);
            var randomCookieTicket = EncryptExtension.CfsEnCode(AppSession.CookieCode);

            //// Set cookie with custid id
            var baseDomain = httpContext.Request.Host.Value;
            httpContext.Response.Cookies.Append(EncryptUrlHelper.UTMS, randomCookieTicket, new CookieOptions
            {
                Domain = baseDomain,
                HttpOnly = true,
                Expires = DateTime.Now.AddDays(1) // Max 24h
            });

            loginResult.Language = LanguageHelper.CheckingLanguageFromCookie();
            loginResult.UTMS = randomCookieTicket;
            loginResult.Domain = baseDomain;

            //// Use for encrypt pwd
            const int EncryptPasswordLength = 4;
            AppSession.EncryptCode = EncryptExtension.RandomNumberString(EncryptPasswordLength);
            loginResult.Captcha = AppSession.EncryptCode;

            return loginResult;
        }

        private bool IsCaptchaRequired(SignInModel model)
            => cacheService.GetLoginUserFailedCount(model.Username) >= commonOptions.LoginFailTimeToStartUsingCaptcha;

        public async Task<SignInResult> SignIn(SignInModel model, HttpContext httpContext)
        {
            var signInResult = new SignInResult
            {
                Ip = ipHelper.ClientIP(),
                LoginTemplate = _loginTheme,
                Username = model.Username,
                Captcha = model.Code,
                NoInteractionTimeInMinutes = commonOptions.NoInteractionTimeInMinutes
            };

            AppSession.EncryptCode = model.Code;

            if (string.IsNullOrEmpty(AppSession.EncryptCode))
            {
                signInResult.ErrCode = SignInErrorContants.InvalidEncryptCode;
                signInResult.ErrMsg = Labels.errInvalidCaptcha;
                return signInResult;
            }

            if (cacheService.IsLoginIPBlocked())
            {
                var ipLockedError = string.Format(
                    Labels.iploginfailedmultipletimeserror,
                    signInResult.Ip,
                    commonOptions.MaxTimesToAllowLoginFail,
                    commonOptions.BlockLoginMinutes);
                signInResult.ErrCode = SignInErrorContants.IPBlocked;
                signInResult.ErrMsg = ipLockedError;
                return signInResult;
            }

            if (IsCaptchaRequired(model))
            {
                var isVerifiedResult = await captchaService.IsVerified(model.CaptchaToken);
                if (isVerifiedResult.IsSuccess)
                {
                    signInResult.IsCaptchaVerified = true;
                }
                else
                {
                    if (captchaOptions.LogEnabled)
                    {
                        var exception = new Exception($"signin captcha: key={model.Username} code={isVerifiedResult.Code}, reason={isVerifiedResult.Reason}, detail={isVerifiedResult.Detail}");
                        logger.Warn(exception.Message);
                    }

                    signInResult.ErrCode = SignInErrorContants.CaptchaRequired;
                    signInResult.ErrMsg = Labels.BotDetectionMessage;
                    return signInResult;
                }
            }

            var password = AesService.Default.Decrypt(model.Password, AppSession.EncryptCode);
            var assetAppPath = AssetAppPathHelper.IsValidAssetAppPath(model.AssetAppPath)
                ? model.AssetAppPath
                : string.Empty;
            VerifyDownlineLogin(model.Username, password, model.DeviceId, model.BrowserSize, signInResult, !string.IsNullOrWhiteSpace(AppSession.DarkModeName), assetAppPath, httpContext);

            return signInResult;
        }

        public SignInResult AutomationTest(string username, string password, string captcha, string deviceId, string browserSize, HttpContext httpContext)
        {
            var signInResult = new SignInResult
            {
                Ip = ipHelper.ClientIP(),
                LoginTemplate = _loginTheme,
                Username = username,
                NoInteractionTimeInMinutes = commonOptions.NoInteractionTimeInMinutes
            };

            if (string.IsNullOrEmpty(username) ||
                string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(captcha) ||
                !automationTestOptions.AllowedIPs.Contains(ipHelper.ClientIP()) ||
                !automationTestOptions.Captcha.Equals(captcha) ||
                !automationTestOptions.AllowUsernameContainWords.All(s => username.ToUpperInvariant().Contains(s)))
            {
                return signInResult;
            }

            VerifyDownlineLogin(username, password, deviceId, browserSize, signInResult, null, string.Empty, httpContext);


            return signInResult;
        }

        private void LogIpBlocked(SignInResult signInResult, HttpContext httpContext)
        {
            var logInfo = new
            {
                Domain = httpContext.Request.Host.Value,
                signInResult.Ip,
                UserAgent = httpContext.Request.Headers["User-Agent"].ToString(),
                Result = signInResult
            };
            logger.Warn($"IP blocked for login: {JsonConvert.SerializeObject(logInfo)}");
        }

        private void UpdateResultAfterLoginFail(
            SignInResult signInResult,
            VerifySignInResult verifySignInResult,
            HttpContext httpContext)
        {
            signInResult.IsChangedPassword = verifySignInResult.LastTimeToChangePassword != DateTime.MinValue;
            if (signInResult.IsChangedPassword)
            {
                signInResult.ChangedPasswordTime = string.Format(Labels.lblPasswordChanged, verifySignInResult.LastTimeToChangePassword);
            }

            var loginIPFailedCount = cacheService.IncreaseLoginIPFailedCount();
            if (loginIPFailedCount > commonOptions.MaxTimesToAllowLoginFail)
            {
                if (loginIPFailedCount == commonOptions.MaxTimesToAllowLoginFail + 1)
                {
                    LogIpBlocked(signInResult, httpContext);
                }

                cacheService.SetLoginIPBlocked(commonOptions.BlockLoginMinutes);
                var ipLockedError = string.Format(
                    Labels.iploginfailedmultipletimeserror,
                    signInResult.Ip,
                    commonOptions.MaxTimesToAllowLoginFail,
                    commonOptions.BlockLoginMinutes);
                signInResult.ErrCode = SignInErrorContants.IPBlocked;
                signInResult.ErrMsg = ipLockedError;
                return;
            }

            var loginUserFailedCount = cacheService.IncreaseLoginUserFailedCount(signInResult.Username, commonOptions.NoInteractionTimeInMinutes);
            if (loginUserFailedCount >= commonOptions.LoginFailTimeToStartUsingCaptcha)
            {
                signInResult.ErrCode = SignInErrorContants.CaptchaRequired;
                Task.Run(async () => await notificationService.NotifyWrongPassword(verifySignInResult.CustId, verifySignInResult.SubAccId, signInResult.Ip));
            }

            signInResult.ErrMsg = verifySignInResult.ErrorMessage;
        }

        private void UpdateResultAfterLoginSuccessfully(
            string password,
            SignInResult signInResult,
            VerifySignInResult verifySignInResult,
            string deviceId,
            string browserSize,
            string assetAppPath,
            HttpContext httpContext)
        {
            cacheService.ResetLoginSession(signInResult.Username);

            var encryptPassword = AesService.Default.Encrypt(password, AppSession.EncryptCode);
            var crossCookieValue = EncryptExtension.CfsEnCode((verifySignInResult.CustId + verifySignInResult.LoginTime.Minute).ToString());

            string url;
            var robotResult = robotDetector.IsRobot(httpContext.Request);
            if (robotResult == 0 || signInResult.IsCaptchaVerified)
            {
                url = EncryptUrlHelper.BuildAfterURL(httpContext, encryptPassword, verifySignInResult, deviceId, browserSize, assetAppPath);
            }
            else
            {
                AppSession.AssignRobotCaptchaSession(
                   verifySignInResult.CustId,
                   verifySignInResult.Username,
                   verifySignInResult.SubAccId,
                   verifySignInResult.SubUserName,
                   verifySignInResult.IsNickname,
                   verifySignInResult.Nickname,
                   encryptPassword,
                   verifySignInResult.SuperId,
                   verifySignInResult.Site,
                   verifySignInResult.RoleId,
                   robotResult.ToString(),
                   deviceId,
                   browserSize,
                   assetAppPath);

                EncryptUrlHelper.WriteLoginLogs(verifySignInResult.SuperId, $"After set sessionId: {httpContext.Session.Id}, custId:{AppSession.CustId}");

                url = "RobotCaptcha/Index";
            }

            var baseDomain = httpContext.Request.Host.Value;
            signInResult.ErrMsg = Labels.lblLoginSuccessful;
            signInResult.AfterLoginUrl = url;
            signInResult.UTMS = crossCookieValue;
            signInResult.Domain = baseDomain;
            signInResult.ErrCode = SignInErrorContants.Success;
            signInResult.Username = verifySignInResult.SubAccId == 0 ? verifySignInResult.Username : signInResult.Username;
            httpContext.Response.Cookies.Append(EncryptUrlHelper.UTMS, crossCookieValue, new CookieOptions
            {
                Domain = baseDomain,
                Expires = DateTime.Now.AddDays(1), // Max 24h
                HttpOnly = true
            });


            if (verifySignInResult.IsKickoutDueToProtectedMode)
            {
                Task.Run(() => kickoutCustomerService.KickoutDueToProtectedMode(
                    verifySignInResult.CustId,
                    verifySignInResult.RoleId,
                    verifySignInResult.ServerId,
                    signInResult.Ip,
                    verifySignInResult.SubAccId));
            }
        }

        private void VerifyDownlineLogin(
            string username,
            string password,
            string deviceId,
            string browserSize,
            SignInResult signInResult,
            bool? isDarkMode,
            string assetAppPath,
            HttpContext httpContext)
        {
            var verifySignInResult = new VerifySignInResult();
            var request = httpContext.Request;
            var cacheIdentity = CacheIdentity.From(
                throttlePolicy.Name,
                AppSetting.SiteGroup,
                new[] { username.Trim().ToLowerInvariant(), ipHelper.ClientIP() });
            var result = netThrottler.Throttle(
                (success) =>
                {
                    var clientIP = ipHelper.ClientIP();
                    var requestInfo = DeviceHelper.GetRequestInfo(request, browserSize, clientIP);
                    requestInfo["countrycode"] = GetCountryCode(clientIP);
                    requestInfo.Add("sitegroup", AppSetting.SiteGroup);
                    verifySignInResult = signInRepository.VerifyDownlineLogin(username, password, requestInfo, isDarkMode);
                    EncryptUrlHelper.WriteLoginLogs(verifySignInResult.SuperId, $"Call DB result: {JsonConvert.SerializeObject(verifySignInResult)}");

                    if (verifySignInResult.ErrorCode == 0)
                    {
                        signInResult.LoginTime = verifySignInResult.LoginTime;

                        UpdateResultAfterLoginSuccessfully(password, signInResult, verifySignInResult, deviceId, browserSize, assetAppPath, httpContext);

                        if (throttlePolicy.WhitelistSuperIds.Contains(verifySignInResult.SuperId))
                        {
                            return ThrottleResult.Allowed;
                        }

                        return success(ThrottleResult.Allowed);
                    }
                    else
                    {
                        UpdateResultAfterLoginFail(signInResult, verifySignInResult, httpContext);

                        return ThrottleResult.Allowed;
                    }
                },
            () => ThrottleResult.Blocked,
            throttlePolicy,
            cacheIdentity);

            if (result == ThrottleResult.Blocked)
            {
                signInResult.ErrCode = SignInErrorContants.LoginTooOften;
                signInResult.ErrMsg = string.Format(Labels.LoginTooOftenBlockedMessage, username);
            }

            EncryptUrlHelper.WriteLoginLogs(verifySignInResult.SuperId, $"After checking login too often: {JsonConvert.SerializeObject(signInResult)}");
        }

        private string GetCountryCode(string ip)
        {
            var countryCode = "Unknown";
            var ipInformation = ipService.GetIPInformation(ip);
            if (ipInformation != null)
            {
                countryCode = ipInformation.CountryLong == "Reserved" ? null : ipInformation.CountryLong;
            }

            return countryCode;
        }
    }
}