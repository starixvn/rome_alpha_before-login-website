// -----------------------------------------------------------------------
// <copyright file="SignInService.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Alpha.Before.Customer.Services.SignIn
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using Cryptography.Aes;
    using Fanex.Alpha.Before.Common.Configurations;
    using Fanex.Alpha.Before.Common.Constants;
    using Fanex.Alpha.Before.Common.Extensions;
    using Fanex.Alpha.Before.Common.Helpers;
    using Fanex.Alpha.Before.Common.Services;
    using Fanex.Alpha.Before.Customer.Configurations;
    using Fanex.Alpha.Before.Customer.Constants;
    using Fanex.Alpha.Before.Customer.Models;
    using Fanex.Alpha.Before.Customer.Repositories.SignIn;
    using Fanex.Alpha.Before.Customer.Resources;
    using Fanex.Alpha.Before.Customer.Services.FPS;
    using Fanex.Logging;
    using Newtonsoft.Json;
    using TelegramBot.Client.Services;

    public interface ISignInService
    {
        SignInResult SignIn(HttpContext httpContext);

        SignInResult SignIn(SignInModel model, HttpContext httpContext);

        SignInResult AutomationTest(string username, string password, string captcha, string deviceId, string browserSize, HttpContext httpContext);
    }

    /// <summary>
    /// Sign in service.
    /// </summary>
    public class SignInService : ISignInService
    {
        private readonly int _maxTimesToAllowLogInFail;
        private readonly int _blockLoginMinute;
        private readonly int _loginFailTimeToStartUsingCaptcha;
        private readonly int _passwordMaxFailTimeForOneUser;
        private readonly int _notInteractionTimeInMinutes;
        private readonly string _baseDomain = string.Empty;
        private readonly string _loginTheme;

        private readonly IPHelper ipHelper;
        private readonly IPService ipService;
        private readonly ISignInRepository _signInRepository;
        private readonly ICacheService cacheService;
        private readonly IRobotDetector robotDetector;
        // private readonly CaptchaOptions captchaOptions;
        // private readonly ICaptchaService captchaService;
        private readonly IKickoutCustomerService kickoutCustomerService;
        private readonly INotificationService notificationService;

        private readonly ILogger logger;

        // private static NetThrottler netThrottler = new NetThrottler();
        // private static ThrottlePolicy loginThrottlePolicy = new ThrottlePolicy(netThrottler.ThrottleSetting.GetKey("Login"));
        // private static IEnumerable<int> whitelistSuperIds = ParseWhitelistSuperIds();

        // static IEnumerable<int> ParseWhitelistSuperIds()
        // {
        //     var custIds = WhitelistConfiguration.Default.CustIds;
        //     return string.IsNullOrWhiteSpace(custIds) ? Enumerable.Empty<int>() : custIds.Split(',').Select(int.Parse).ToList();
        // }

        public SignInService(
            ILogger logger,
            IPHelper ipHelper,
            ISignInRepository signInRepository,
            IRobotDetector robotDetector,
            ICacheService cacheService,
            // CaptchaOptions captchaOptions,
            // ICaptchaService captchaService,
            IKickoutCustomerService kickoutCustomerService,
            IPService ipService,
            INotificationService notificationService)
        {
            this.logger = logger;
            this.ipHelper = ipHelper;
            this.ipService = ipService;
            _signInRepository = signInRepository;
            this.cacheService = cacheService;
            this.robotDetector = robotDetector;
            // this.captchaOptions = captchaOptions;
            // this.captchaService = captchaService;
            this.kickoutCustomerService = kickoutCustomerService;
            this.notificationService = notificationService;
            // var _loginThemeSection = LoginThemeSection.Instance;

            // _loginTheme = _loginThemeSection.DefaultTheme;
            // if (DateTime.Now >= Converter.ToDateTime(_loginThemeSection.StartDateShowEventTheme) &&
            //     DateTime.Now <= Converter.ToDateTime(_loginThemeSection.EndDateShowEventTheme))
            // {
            //     _loginTheme = _loginThemeSection.EventTheme;
            // }

            // var currentLoginUrl = UrlHelper.SiteGroupSetting.BeforeUrl;
            // _baseDomain = new Uri(currentLoginUrl).Host;
            // var alphaCommonSettings = AlphaCommonSection.Instance;
            // if (alphaCommonSettings != null)
            // {
            //     _maxTimesToAllowLogInFail = alphaCommonSettings.MaxTimesToAllowLoginFail;
            //     _blockLoginMinute = alphaCommonSettings.BlockLoginMinutes;
            //     _loginFailTimeToStartUsingCaptcha = alphaCommonSettings.LoginFailTimeToStartUsingCaptcha;
            //     _notInteractionTimeInMinutes = alphaCommonSettings.NoInteractionTimeInMinutes;
            //     _passwordMaxFailTimeForOneUser = alphaCommonSettings.PasswordMaxFailTimeForOneUser;
            // }


        }

        public SignInResult SignIn(HttpContext httpContext)
        {
            var loginResult = new SignInResult
            {
                LoginTemplate = _loginTheme,
                Ip = ipHelper.ClientIP(),
                Username = string.Empty
            };


            try
            {
                //// Check cross cookie compatibility
                const int EncryptCookieCodeLength = 6;
                AppSession.CookieCode = EncryptExtension.RandomNumberString(EncryptCookieCodeLength);
                var randomCookieTicket = EncryptExtension.CfsEnCode(AppSession.CookieCode);

                try
                {
                    //// Set cookie with custid id
                    HttpCookie cookie = new(EncryptURLHelper.UTMS, randomCookieTicket)
                    {
                        Domain = _baseDomain,
                        HttpOnly = true,
                        Expires = DateTime.Now.AddDays(1) // Max 24h
                    };
                    httpContext.Response.SetCookie(cookie);
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message, ex);
                }

                loginResult.Language = LanguageHelper.CheckingLanguageFromCookie();
                loginResult.UTMS = randomCookieTicket;
                loginResult.Domain = _baseDomain;

                //// Use for encrypt pwd
                const int EncryptPasswordLength = 4;
                AppSession.EncryptCode = EncryptExtension.RandomNumberString(EncryptPasswordLength);
                loginResult.Captcha = AppSession.EncryptCode;
            }
            catch (Exception ex)
            {
                loginResult.ErrCode = ErrorCodeConstants.ServerError;
                loginResult.ErrMsg = AppSetting.IsCustomErrorEnabled ?
                    string.Format(Infrastructure.Resources.Labels.Error_Server, 5004) : ex.ToString();
                logger.Error(ex.Message, ex);
            }

            //loginResult.GoogleReCaptcha = UrlHelper.SiteGroupSetting.GoogleReCaptcha;

            return loginResult;
        }

        private bool IsCaptchaRequired(SignInModel model)
        {
            return IsShowReCaptcha(model.Username);
        }

        public SignInResult SignIn(SignInModel model, HttpContext httpContext)
        {
            var signInResult = new SignInResult
            {
                Ip = ipHelper.ClientIP(),
                LoginTemplate = _loginTheme,
                Username = model.Username,
                GoogleReCaptcha = UrlHelper.SiteGroupSetting.GoogleReCaptcha,
                Captcha = model.Code
            };

            try
            {
                AppSession.EncryptCode = model.Code;

                if (string.IsNullOrEmpty(AppSession.EncryptCode))
                {
                    signInResult.ErrCode = SignInErrorContants.InvalidEncryptCode;
                    signInResult.ErrMsg = Labels.errInvalidCaptcha;
                    return signInResult;
                }

                if (cacheService.IsLoginIPBlocked())
                {
                    var ipLockedError = string.Format(Labels.iploginfailedmultipletimeserror, signInResult.Ip, _maxTimesToAllowLogInFail, _blockLoginMinute);
                    signInResult.ErrCode = SignInErrorContants.IPBlocked;
                    signInResult.ErrMsg = ipLockedError;
                    return signInResult;
                }

                // if (IsCaptchaRequired(model))
                // {
                //     var isVerifiedResult = captchaService.IsVerfied(model.CaptchaToken);
                //     if (isVerifiedResult.IsSuccess)
                //     {
                //         signInResult.IsCaptchaVerified = true;
                //     }
                //     else
                //     {
                //         if (captchaOptions.LogEnabled)
                //         {
                //             var exception = new Exception($"signin captcha: key={model.Username} code={isVerifiedResult.Code}, reason={isVerifiedResult.Reason}, detail={isVerifiedResult.Detail}");
                //             logger.Warn(exception.Message);
                //         }

                //         signInResult.ErrCode = SignInErrorContants.CaptchaRequired;
                //         signInResult.ErrMsg = Labels.BotDetectionMessage;
                //         return signInResult;
                //     }
                // }

                var password = AesService.Default.Decrypt(model.Password, AppSession.EncryptCode);
                var assetAppPath = AssetAppPathHelper.IsValidAssetAppPath(model.AssetAppPath)
                    ? model.AssetAppPath
                    : string.Empty;
                VerifyDownlineLogin(model.Username, password, model.DeviceId, model.BrowserSize, signInResult, !string.IsNullOrWhiteSpace(AppSession.DarkModeName), assetAppPath, httpContext);
            }
            catch (Exception ex)
            {
                signInResult.ErrCode = ErrorCodeConstants.ServerError;
                signInResult.ErrMsg = AppSetting.IsCustomErrorEnabled ?
                    string.Format(Infrastructure.Resources.Labels.Error_Server, 5004) : ex.ToString();

                logger.Error(ex.Message, ex);
            }

            return signInResult;
        }

        public SignInResult AutomationTest(string username, string password, string captcha, string deviceId, string browserSize, HttpContext httpContext)
        {
            var signInResult = new SignInResult
            {
                Ip = ipHelper.ClientIP(),
                LoginTemplate = _loginTheme,
                Username = username
            };

            try
            {
                var automationTestSettings = AutomationTestSettings.Instance;
                if (automationTestSettings == null)
                {
                    return signInResult;
                }

                var allowCaptcha = automationTestSettings.Captcha;
                var allowIps = automationTestSettings.AllowsIPsToAccess;
                var allowUsernameContainWords = automationTestSettings.AllowUsernameContainWords;

                if (string.IsNullOrEmpty(allowCaptcha) ||
                    string.IsNullOrEmpty(allowIps) ||
                    string.IsNullOrEmpty(allowUsernameContainWords) ||
                    string.IsNullOrEmpty(username) ||
                    string.IsNullOrEmpty(password) ||
                    string.IsNullOrEmpty(captcha))
                {
                    return signInResult;
                }

                var allowWords = allowUsernameContainWords.ToUpperInvariant().Split(',');
                if (!allowIps.Split(',').Contains(ipHelper.ClientIP()) ||
                    !allowCaptcha.Equals(captcha) ||
                    !allowWords.All(s => username.ToUpperInvariant().Contains(s)))
                {
                    return signInResult;
                }

                VerifyDownlineLogin(username, password, deviceId, browserSize, signInResult, null, string.Empty, httpContext);
            }
            catch (Exception ex)
            {
                signInResult.ErrCode = ErrorCodeConstants.ServerError;
                signInResult.ErrMsg = AppSetting.IsCustomErrorEnabled ?
                    string.Format(Infrastructure.Resources.Labels.Error_Server, 5004) : ex.ToString();
                logger.Error(ex.Message, ex);
            }

            return signInResult;
        }

        private bool IsShowReCaptcha(string username)
        {
            return cacheService.GetLoginUserFailedCount(username) >= _loginFailTimeToStartUsingCaptcha;
        }

        private void LogIpBlocked(SignInResult signInResult, HttpContext httpContext)
        {
            var logInfo = new
            {
                Domain = httpContext.Request.Headers["Host"],
                signInResult.Ip,
                httpContext.Request.UserAgent,
                Result = signInResult
            };
            logger.Warn($"IP blocked for login: {JsonConvert.SerializeObject(logInfo)}");
        }

        private void UpdateResultAfterLoginFail(
            SignInResult signInResult,
            VerifySignInResult verifySignInResult, HttpContext httpContext)
        {
            signInResult.IsChangedPassword = verifySignInResult.LastTimeToChangePassword != DateTime.MinValue;
            if (signInResult.IsChangedPassword)
            {
                signInResult.ChangedPasswordTime = string.Format(Labels.lblPasswordChanged, verifySignInResult.LastTimeToChangePassword);
            }

            var loginIPFailedCount = cacheService.IncreaseLoginIPFailedCount();
            if (loginIPFailedCount > _maxTimesToAllowLogInFail)
            {
                if (loginIPFailedCount == _maxTimesToAllowLogInFail + 1)
                {
                    LogIpBlocked(signInResult, httpContext);
                }

                cacheService.SetLoginIPBlocked(_blockLoginMinute);
                var ipLockedError = string.Format(Labels.iploginfailedmultipletimeserror, signInResult.Ip, _maxTimesToAllowLogInFail, _blockLoginMinute);
                signInResult.ErrCode = SignInErrorContants.IPBlocked;
                signInResult.ErrMsg = ipLockedError;
                return;
            }

            var loginUserFailedCount = cacheService.IncreaseLoginUserFailedCount(signInResult.Username, _notInteractionTimeInMinutes);
            if (loginUserFailedCount >= _loginFailTimeToStartUsingCaptcha)
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
                url = EncryptURLHelper.BuildAfterURL(httpContext, encryptPassword, verifySignInResult, deviceId, browserSize, assetAppPath);
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

                WriteLoginLogs(verifySignInResult.SuperId, $"After set sessionId: {httpContext.Session.SessionID}, custId:{AppSession.CustId}");

                url = "RobotCaptcha/Index";
            }

            signInResult.ErrMsg = Labels.lblLoginSuccessful;
            signInResult.AfterLoginUrl = url;
            signInResult.UTMS = crossCookieValue;
            signInResult.Domain = _baseDomain;
            signInResult.ErrCode = SignInErrorContants.Success;
            signInResult.Username = verifySignInResult.SubAccId == 0 ? verifySignInResult.Username : signInResult.Username;

            try
            {
                // Set cookie with custid id and seed
                var cookie = new HttpCookie(EncryptURLHelper.UTMS, crossCookieValue)
                {
                    Domain = _baseDomain,
                    Expires = DateTime.Now.AddDays(1), // Max 24h
                    HttpOnly = true
                };
                httpContext.Response.SetCookie(cookie);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }

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
            // var cacheIdentity = CacheIdentity.From(
            //     loginThrottlePolicy.Name,
            //     UrlHelper.SiteGroupSetting.SiteGroup,
            //     new[] { username.Trim().ToLowerInvariant(), request.GetClientIp().ToString() });
            // var result = netThrottler.Throttle(
            //     (success) =>
            //     {
            var clientIP = ipHelper.ClientIP();
            var requestInfo = DeviceHelper.GetRequestInfo(request, browserSize, clientIP);
            requestInfo["countrycode"] = GetCountryCode(clientIP);
            requestInfo.Add("sitegroup", UrlHelper.SiteGroupSetting.SiteGroup);
            verifySignInResult = _signInRepository.VerifyDownlineLogin(username, password, requestInfo, isDarkMode);
            WriteLoginLogs(verifySignInResult.SuperId, $"Call DB result: {JsonConvert.SerializeObject(verifySignInResult)}");

            if (verifySignInResult.ErrorCode == 0)
            {
                signInResult.LoginTime = verifySignInResult.LoginTime;

                UpdateResultAfterLoginSuccessfully(password, signInResult, verifySignInResult, deviceId, browserSize, assetAppPath, httpContext);

                // if (whitelistSuperIds.Contains(verifySignInResult.SuperId))
                // {
                //     return ThrottleResult.Allowed;
                // }

                // return success(ThrottleResult.Allowed);
            }
            else
            {
                UpdateResultAfterLoginFail(signInResult, verifySignInResult, httpContext);

                //return ThrottleResult.Allowed;
            }
            // },
            // () => ThrottleResult.Blocked,
            // loginThrottlePolicy,
            // cacheIdentity
            // );

            // if (result == ThrottleResult.Blocked)
            // {
            //     signInResult.ErrCode = SignInErrorContants.LoginTooOften;
            //     signInResult.ErrMsg = string.Format(Labels.LoginTooOftenBlockedMessage, username);
            // }

            WriteLoginLogs(verifySignInResult.SuperId, $"After checking login too often: {JsonConvert.SerializeObject(signInResult)}");
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

        private void WriteLoginLogs(int superId, string message)
        {
            if (SuperIds.Any(id => id == superId))
            {
                logger.Warn(message);
            }
        }

        private static IEnumerable<int> SuperIds =
            string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["Robot:TrackLogSuperIds"]) ? Enumerable.Empty<int>() :
            ConfigurationManager.AppSettings["Robot:TrackLogSuperIds"]
            .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse);
    }
}