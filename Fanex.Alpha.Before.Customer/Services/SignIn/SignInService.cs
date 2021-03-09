// -----------------------------------------------------------------------
// <copyright file="SignInService.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Alpha.Before.Customer.Services.SignIn
{
    using Cryptography.Aes;
    using Fanex.Alpha.Before.Common.Configurations;
    using Fanex.Alpha.Before.Common.Extensions;
    using Fanex.Alpha.Before.Common.Helpers;
    using Fanex.Alpha.Before.Customer.Configurations;
    using Fanex.Alpha.Before.Customer.Constants;
    using Fanex.Alpha.Before.Customer.Models;
    using Fanex.Alpha.Before.Customer.Repositories.SignIn;
    using Fanex.Alpha.Before.Customer.Resources;
    using Fanex.Alpha.Before.Customer.Services.FPS;
    using Fanex.Alpha.Before.Infrastructure.Constants;
    using Fanex.Common;
    using global::Alpha.NetThrottling;
    using global::Alpha.NetThrottling.Models;
    using global::Alpha.NetThrottling.Utils;
    using Logging.Extensions.Web;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web;
    using InfrastructureLanguages = Infrastructure.Resources;
    using LoginLanguages = Resources;

    public interface ISignInService
    {
        SignInResult SignIn();

        SignInResult SignInWithRecaptcha(string username, string password, string language, bool isValidCaptcha, string code, string deviceId, string browserSize);

        SignInResult AutomationTest(string username, string password, string captcha, string deviceId, string browserSize);

        bool IsShowReCaptcha();
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

        private readonly ISignInRepository _signInRepository;
        private readonly ICacheService cacheService;
        private readonly IRobotDetector robotDetector;
        
        private readonly Logging.ILogger logger;
        private readonly HttpContext httpContext;

        private static NetThrottler netThrottler = new NetThrottler();
        private static ThrottlePolicy loginThrottlePolicy = new ThrottlePolicy(netThrottler.ThrottleSetting.GetKey("Login"));
        private static IEnumerable<int> whitelistSuperIds = ParseWhitelistSuperIds();

        static IEnumerable<int> ParseWhitelistSuperIds()
        {
            var custIds = WhitelistConfiguration.Default.CustIds;
            return string.IsNullOrWhiteSpace(custIds) ? Enumerable.Empty<int>() : custIds.Split(',').Select(int.Parse).ToList();
        }

        public SignInService() :
            this(null, null, null, null, null)
        {
        }

        public SignInService(SignInRepository signInRepository,
            IRobotDetector robotDetector,
            Logging.ILogger logger,
            HttpContext httpContext,
            ICacheService cacheService)
        {
            _signInRepository = signInRepository ?? new SignInRepository();
            this.cacheService = cacheService ?? new CacheService();
            var _loginThemeSection = LoginThemeSection.Instance;

            _loginTheme = _loginThemeSection.DefaultTheme;
            if (DateTime.Now >= Converter.ToDateTime(_loginThemeSection.StartDateShowEventTheme) &&
                DateTime.Now <= Converter.ToDateTime(_loginThemeSection.EndDateShowEventTheme))
            {
                _loginTheme = _loginThemeSection.EventTheme;
            }

            var currentLoginUrl = UrlHelper.SiteGroupSetting.BeforeUrl;
            _baseDomain = new Uri(currentLoginUrl).Host;
            var alphaCommonSettings = AlphaCommonSection.Instance;
            if (alphaCommonSettings != null)
            {
                _maxTimesToAllowLogInFail = alphaCommonSettings.MaxTimesToAllowLoginFail;
                _blockLoginMinute = alphaCommonSettings.BlockLoginMinutes;
                _loginFailTimeToStartUsingCaptcha = alphaCommonSettings.LoginFailTimeToStartUsingCaptcha;
                _notInteractionTimeInMinutes = alphaCommonSettings.NoInteractionTimeInMinutes;
                _passwordMaxFailTimeForOneUser = alphaCommonSettings.PasswordMaxFailTimeForOneUser;
            }

            this.robotDetector = robotDetector ?? new RobotDetector();
            this.logger = logger ?? Logging.Logger.Log;
            this.httpContext = httpContext ?? HttpContext.Current;
        }

        /// <summary>
        /// Sign In default method.
        /// </summary>
        /// <returns>Return result.</returns>
        public SignInResult SignIn()
        {
            var loginResult = new SignInResult
            {
                LoginTemplate = _loginTheme,
                Ip = IPHelper.ClientIP(),
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
                    HttpCookie cookie = new HttpCookie(EncryptURLHelper.UTMS, randomCookieTicket)
                    {
                        Domain = _baseDomain,
                        HttpOnly = true,
                        Expires = DateTime.Now.AddDays(1) // Max 24h
                    };
                    httpContext.Response.SetCookie(cookie);
                }
                catch (Exception ex)
                {
                    ExceptionHandler.Handle(ex);
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
                loginResult.ErrMsg = HttpContext.Current.Request.RequestContext.HttpContext.IsCustomErrorEnabled ?
                    Infrastructure.Resources.Labels.Error_Server : ex.ToString();
                ExceptionHandler.Handle(ex);
            }

            loginResult.GoogleReCaptcha = UrlHelper.SiteGroupSetting.GoogleReCaptcha;

            return loginResult;
        }

        /// <summary>
        /// Sign In method.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">User's password.</param>
        /// <param name="language">Current language.</param>
        /// <param name="isValidCaptcha">The captcha.</param>
        /// <param name="code">The code string.</param>
        /// <returns>Return login result.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public SignInResult SignInWithRecaptcha(
            string username,
            string password,
            string language,
            bool isValidCaptcha,
            string code,
            string deviceId,
            string browserSize)
        {
            var signInResult = new SignInResult
            {
                Ip = IPHelper.ClientIP(),
                LoginTemplate = _loginTheme,
                Username = username,
                GoogleReCaptcha = UrlHelper.SiteGroupSetting.GoogleReCaptcha
            };

            try
            {
                AppSession.EncryptCode = code;

                if (cacheService.GetTotallyLoginFailed() > _maxTimesToAllowLogInFail)
                {
                    if (cacheService.GetLoginFailedTime().AddMinutes(_blockLoginMinute).CompareTo(DateTime.UtcNow) > 0)
                    {
                        cacheService.SetLoginFailedTime();
                        var ipLockedError = string.Format(Labels.iploginfailedmultipletimeserror, signInResult.Ip, _maxTimesToAllowLogInFail, _blockLoginMinute);
                        signInResult.ErrCode = SignInErrorContants.ErrorInvalidLogin;
                        signInResult.ErrMsg = ipLockedError;

                        return signInResult;
                    }
                    else
                    {
                        cacheService.ResetLoginFailed();
                    }
                }

                // If login with no action for the laset 5 minutes, login without captcha
                var lastLoginUsername = cacheService.GetLastLoginUsername();
                if (!string.IsNullOrEmpty(lastLoginUsername) &&
                    !lastLoginUsername.Equals(username, StringComparison.OrdinalIgnoreCase))
                {
                    if (IsShowReCaptcha() && !isValidCaptcha)
                    {
                        cacheService.ResetCurrentLoginFailed();
                        cacheService.SetLastLoginUsername(username);

                        signInResult.ErrCode = SignInErrorContants.Error2; ////no need to write log in fps and redirect to login page.
                        signInResult.Captcha = AppSession.EncryptCode;

                        return signInResult;
                    }

                    cacheService.ResetCurrentLoginFailed();
                }

                // If no action for the laset 5 minutes, login without captcha
                var loginFailedUtcTime = cacheService.GetLoginFailedTime();
                if (loginFailedUtcTime != DateTime.MinValue &&
                    loginFailedUtcTime.AddMinutes(_notInteractionTimeInMinutes).CompareTo(DateTime.UtcNow) < 0)
                {
                    cacheService.ResetCurrentLoginFailed();
                }

                if (string.IsNullOrEmpty(AppSession.EncryptCode))
                {
                    signInResult.ErrCode = SignInErrorContants.ErrorInvalidCaptcha;
                    signInResult.ErrMsg = LoginLanguages.Labels.errInvalidCaptcha;

                    return signInResult;
                }
                else
                {
                    signInResult.Captcha = AppSession.EncryptCode;
                }

                if (IsShowReCaptcha() && !isValidCaptcha)
                {
                    signInResult.ErrCode = SignInErrorContants.ErrorInvalidCaptcha;
                    signInResult.ErrMsg = Labels.errInvalidCaptcha;
                    var loginFailedCount = cacheService.IncreaseLoginFailedCount();
                    if (loginFailedCount == _maxTimesToAllowLogInFail)
                    {
                        LogIpBlocked(signInResult);
                    }

                    return signInResult;
                }

                try
                {
                    password = AesExtension.Decrypt(password, AppSession.EncryptCode);
                }
                catch
                {
                    signInResult.ErrCode = ErrorCodeConstants.ServerError;
                    signInResult.ErrMsg = string.Empty;
                    return signInResult;
                }

                VerifyDownlineLogin(username, password, deviceId, browserSize, signInResult);
            }
            catch (Exception ex)
            {
                signInResult.ErrCode = ErrorCodeConstants.ServerError;
                signInResult.ErrMsg = HttpContext.Current.Request.RequestContext.HttpContext.IsCustomErrorEnabled ?
                    InfrastructureLanguages.Labels.Error_Server : ex.ToString();
                ExceptionHandler.Handle(ex);
            }

            cacheService.SetLastLoginUsername(username);

            return signInResult;
        }

        /// <summary>
        /// Support for automation test tool.
        /// </summary>
        /// <param name="username">Username login.</param>
        /// <param name="password">User's password.</param>
        /// <param name="captcha">Captcha value.</param>
        /// <returns>Return result.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public SignInResult AutomationTest(string username, string password, string captcha, string deviceId, string browserSize)
        {
            var signInResult = new SignInResult
            {
                Ip = IPHelper.ClientIP(),
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
                if (!allowIps.Split(',').Contains(IPHelper.ClientIP()) ||
                    !allowCaptcha.Equals(captcha) ||
                    !allowWords.All(s => username.ToUpperInvariant().Contains(s)))
                {
                    return signInResult;
                }

                VerifyDownlineLogin(username, password, deviceId, browserSize, signInResult);
            }
            catch (Exception ex)
            {
                signInResult.ErrCode = ErrorCodeConstants.ServerError;
                signInResult.ErrMsg = HttpContext.Current.Request.RequestContext.HttpContext.IsCustomErrorEnabled ?
                    InfrastructureLanguages.Labels.Error_Server : ex.ToString();
                ExceptionHandler.Handle(ex);
            }

            return signInResult;
        }

        public bool IsShowReCaptcha()
        {
            return cacheService.GetCurrentLoginFailed() >= _loginFailTimeToStartUsingCaptcha;
        }

        private void LogIpBlocked(SignInResult signInResult)
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

        /// <summary>
        /// Update result login after login Fail.
        /// </summary>
        /// <param name="signInResult">Sign In result.</param>
        /// <param name="verifySignInResult">Verify sign in result.</param>
        private void UpdateResultAfterLoginFail(
            SignInResult signInResult,
            VerifySignInResult verifySignInResult)
        {
            signInResult.IsChangedPassword = verifySignInResult.LastTimeToChangePassword != DateTime.MinValue;
            if (signInResult.IsChangedPassword)
            {
                signInResult.ChangedPasswordTime = string.Format(Labels.lblPasswordChanged, verifySignInResult.LastTimeToChangePassword);
            }

            cacheService.SetLoginFailedTime();
            var loginFailedCount = cacheService.IncreaseLoginFailedCount();
            if (loginFailedCount > _maxTimesToAllowLogInFail)
            {
                var ipLockedError = string.Format(Labels.iploginfailedmultipletimeserror, signInResult.Ip, _maxTimesToAllowLogInFail, _blockLoginMinute);
                signInResult.ErrCode = SignInErrorContants.ErrorInvalidLogin;
                signInResult.ErrMsg = ipLockedError;
            }
            else
            {
                signInResult.ErrCode = verifySignInResult.ErrorCode;
                signInResult.ErrMsg = verifySignInResult.ErrorMessage;
                if (loginFailedCount == _maxTimesToAllowLogInFail)
                {
                    LogIpBlocked(signInResult);
                }
            }
        }

        /// <summary>
        /// Update result login after login successfully.
        /// </summary>
        /// <param name="password">User password.</param>
        /// <param name="signInResult">Login result.</param>
        /// <param name="verifySignInResult">Verify sign in result.</param>
        private void UpdateResultAfterLoginSuccessfully(
            string password,
            SignInResult signInResult,
            VerifySignInResult verifySignInResult,
            string deviceId,
            string browserSize)
        {
            cacheService.ResetLoginFailed();

            var encryptPassword = AesExtension.Encrypt(password, AppSession.EncryptCode);
            var crossCookieValue = EncryptExtension.CfsEnCode((verifySignInResult.CustId + verifySignInResult.LoginTime.Minute).ToString());

            string url;
            var robotResult = robotDetector.IsRobot(httpContext.Request);

            if (robotResult == 0)
            {
                url = EncryptURLHelper.BuildAfterURL(httpContext, encryptPassword, verifySignInResult, deviceId, browserSize);
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
                   browserSize);

                WriteLoginLogs(verifySignInResult.SuperId, $"After set sessionId: {httpContext.Session.SessionID}, custId:{AppSession.CustId}");

                url = "RobotCaptcha/Index";
            }

            signInResult.ErrMsg = LoginLanguages.Labels.lblLoginSuccessful;
            signInResult.AfterLoginUrl = url;
            signInResult.UTMS = crossCookieValue;
            signInResult.Domain = _baseDomain;
            signInResult.ErrCode = -1;
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
                ExceptionHandler.Handle(ex);
            }
        }

        private void VerifyDownlineLogin(
            string username,
            string password,
            string deviceId,
            string browserSize,
            SignInResult signInResult)
        {
            var verifySignInResult = new VerifySignInResult();
            var request = httpContext.Request;
            var cacheIdentity = CacheIdentity.From(
                loginThrottlePolicy.Name,
                UrlHelper.SiteGroupSetting.SiteGroup,
                new[] { username.Trim().ToLowerInvariant(), request.GetClientIp().ToString() });
            var result = netThrottler.Throttle(
                (success) =>
                {
                    var requestInfo = DeviceHelper.GetRequestInfo(request, browserSize);
                    requestInfo.Add("sitegroup", UrlHelper.SiteGroupSetting.SiteGroup);
                    verifySignInResult = _signInRepository.VerifyDownlineLogin(username, password, requestInfo);
                    WriteLoginLogs(verifySignInResult.SuperId, $"Call DB result: {JsonConvert.SerializeObject(verifySignInResult)}");

                    if (verifySignInResult.ErrorCode == 0)
                    {
                        UpdateResultAfterLoginSuccessfully(password, signInResult, verifySignInResult, deviceId, browserSize);
                        if (whitelistSuperIds.Contains(verifySignInResult.SuperId))
                        {
                            return ThrottleResult.Allowed;
                        }

                        return success(ThrottleResult.Allowed);
                    }
                    else
                    {
                        UpdateResultAfterLoginFail(signInResult, verifySignInResult);
                        return ThrottleResult.Allowed;
                    }
                },
                () => ThrottleResult.Blocked,
                loginThrottlePolicy,
                cacheIdentity
                );

            if (result == ThrottleResult.Blocked)
            {
                signInResult.ErrCode = SignInErrorContants.Error2;
                signInResult.ErrMsg = string.Format(Labels.LoginTooOftenBlockedMessage, username);
            }

            WriteLoginLogs(verifySignInResult.SuperId, $"After checking login too often: {JsonConvert.SerializeObject(signInResult)}");
        }

        private static void WriteLoginLogs(int superId, string message)
        {
            if (SuperIds.Any(id => id == superId))
            {
                Logging.Logger.Log.Warn(message);
            }
        }

        private static IEnumerable<int> SuperIds =
            string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["Robot:TrackLogSuperIds"]) ? Enumerable.Empty<int>() :
            ConfigurationManager.AppSettings["Robot:TrackLogSuperIds"]
            .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse);
    }
}