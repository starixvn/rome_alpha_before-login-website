// -----------------------------------------------------------------------
// <copyright file="SignInService.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Alpha.Before.Customer.Services.SignIn
{
    using Cryptography.Aes;
    using Fanex.Alpha.Before.Common.Configurations;
    using Fanex.Alpha.Before.Common.Constants;
    using Fanex.Alpha.Before.Common.Extensions;
    using Fanex.Alpha.Before.Common.Helpers;
    using Fanex.Alpha.Before.Customer.Configurations;
    using Fanex.Alpha.Before.Customer.Constants;
    using Fanex.Alpha.Before.Customer.Models;
    using Fanex.Alpha.Before.Customer.Repositories.SignIn;
    using Fanex.Alpha.Before.Customer.Resources;
    using Fanex.Alpha.Before.Customer.Services.FPS;
    using Fanex.Alpha.Before.Infrastructure.Constants;
    using Fanex.Alpha.Before.Infrastructure.Extensions;
    using Fanex.Common;
    using global::Alpha.NetThrottling;
    using global::Alpha.NetThrottling.Models;
    using global::Alpha.NetThrottling.Utils;
    using global::FPS.Browserless.Detection.Client;
    using Logging.Extensions.Web;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web;
    using InfrastructureLanguages = Fanex.Alpha.Before.Infrastructure.Resources;
    using LoginLanguages = Fanex.Alpha.Before.Customer.Resources;

    /// <summary>
    /// Sign in service.
    /// </summary>
    public class SignInService : ISignInService
    {
        private static ThrottlePolicy configurationLoginPolicy = Mapper.From(NetThrottlingConfiguration.Current.Keys["Login"]);

        private readonly int _maxTimesToAllowLogInFail;
        private readonly int _blockLoginMinute;
        private readonly int _loginFailTimeToStartUsingCaptcha;
        private readonly int _passwordMaxFailTimeForOneUser;
        private readonly int _notInteractionTimeInMinutes;
        private readonly string _baseDomain = string.Empty;
        private readonly string _loginTheme;

        private readonly ISignInRepository _signInRepository;
        private readonly string _totalLoginFailedSessionKey = string.Format(SessionConstants.TOTALLOGINFAILEDCOUNT, IPHelper.ClientIP());
        private readonly string _currentLoginFailedSessionKey = string.Format(SessionConstants.CURRENTLOGINFAILEDCOUNT, IPHelper.ClientIP());
        private readonly string _latestLoginFailSessionKey = string.Format(SessionConstants.LOGINFAILEDTIME, IPHelper.ClientIP());
        private readonly string _lastLoginUserNameSessionKey = string.Format(SessionConstants.LASTLOGINUSERNAME, IPHelper.ClientIP());

        private readonly IRobotDetector robotDetector;
        private readonly ThrottlePolicy loginPolicy;
        private readonly NetThrottler netThrottler;
        private readonly Logging.ILogger logger;
        private readonly HttpContext httpContext;

        private static IEnumerable<int> whitelistSuperIds = ParseWhitelistSuperIds();

        static IEnumerable<int> ParseWhitelistSuperIds()
        {
            var custIds = WhitelistConfiguration.Current.CustIds;
            return string.IsNullOrWhiteSpace(custIds) ? Enumerable.Empty<int>() : custIds.Split(',').Select(int.Parse).ToList();
        }

        public SignInService() :
            this(null, null, null, null, null, null)
        {
        }

        public SignInService(SignInRepository signInRepository, IRobotDetector robotDetector, NetThrottler netThrottler, ThrottlePolicy loginPolicy, Logging.ILogger logger, HttpContext httpContext)
        {
            _signInRepository = signInRepository ?? new SignInRepository();
            var _loginThemeSection = LoginThemeSection.Instance;

            this._loginTheme = _loginThemeSection.DefaultTheme;
            if (DateTime.Now >= Converter.ToDateTime(_loginThemeSection.StartDateShowEventTheme) &&
                DateTime.Now <= Converter.ToDateTime(_loginThemeSection.EndDateShowEventTheme))
            {
                this._loginTheme = _loginThemeSection.EventTheme;
            }

            var currentLoginUrl = UrlHelper.SiteGroupSetting.BeforeUrl;
            this._baseDomain = new Uri(currentLoginUrl).Host;
            var alphaCommonSettings = AlphaCommonSection.Instance;
            if (alphaCommonSettings != null)
            {
                this._maxTimesToAllowLogInFail = alphaCommonSettings.MaxTimesToAllowLoginFail;
                this._blockLoginMinute = alphaCommonSettings.BlockLoginMinutes;
                this._loginFailTimeToStartUsingCaptcha = alphaCommonSettings.LoginFailTimeToStartUsingCaptcha;
                this._notInteractionTimeInMinutes = alphaCommonSettings.NoInteractionTimeInMinutes;
                this._passwordMaxFailTimeForOneUser = alphaCommonSettings.PasswordMaxFailTimeForOneUser;
            }

            this.robotDetector = robotDetector ?? new RobotDetector();
            this.netThrottler = netThrottler ?? new NetThrottler();
            this.loginPolicy = loginPolicy ?? configurationLoginPolicy;
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
                        Domain = this._baseDomain,
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
                loginResult.Domain = this._baseDomain;

                //// Use for encrypt pwd
                const int EncryptPasswordLength = 4;
                AppSession.EncryptCode = EncryptExtension.RandomNumberString(EncryptPasswordLength);
                loginResult.Captcha = AppSession.EncryptCode;
            }
            catch (Exception ex)
            {
                loginResult.ErrCode = ErrorCodeConstants.ServerError;
                loginResult.ErrMsg = Infrastructure.Resources.Labels.Error_Server;
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

                if (this.TotallyLoginFailed() > _maxTimesToAllowLogInFail)
                {
                    if (this.LoginFailedTime().AddMinutes(_blockLoginMinute).CompareTo(DateTime.Now) > 0)
                    {
                        this.SetLoginFailedTime();
                        var ipLockedError = string.Format(Labels.iploginfailedmultipletimeserror, signInResult.Ip, _maxTimesToAllowLogInFail, _blockLoginMinute);
                        signInResult.ErrCode = SignInErrorContants.ErrorInvalidLogin;
                        signInResult.ErrMsg = ipLockedError;

                        return signInResult;
                    }
                    else
                    {
                        this.ResetLoginFailed();
                    }
                }

                // If login with no action for the laset 5 minutes, login without captcha
                if (!string.IsNullOrEmpty(this.LastLoginUserName()) &&
                    !this.LastLoginUserName().Equals(username, StringComparison.OrdinalIgnoreCase))
                {
                    if (IsShowReCaptcha() && !isValidCaptcha)
                    {
                        ResetCurrentLoginFailed();
                        CacheExtension.Set(this._lastLoginUserNameSessionKey, username);

                        signInResult.ErrCode = SignInErrorContants.Error2; ////no need to write log in fps and redirect to login page.
                        signInResult.Captcha = AppSession.EncryptCode;

                        return signInResult;
                    }

                    ResetCurrentLoginFailed();
                }

                // If no action for the laset 5 minutes, login without captcha
                if (this.LoginFailedTime() != DateTime.MinValue &&
                    this.LoginFailedTime().AddMinutes(_notInteractionTimeInMinutes).CompareTo(DateTime.Now) < 0)
                {
                    ResetCurrentLoginFailed();
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
                    var loginFailedCount = this.IncreaseLoginFailedCount();
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
                signInResult.ErrMsg = InfrastructureLanguages.Labels.Error_Server;
                ExceptionHandler.Handle(ex);
            }

            CacheExtension.Set(this._lastLoginUserNameSessionKey, username);

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
                signInResult.ErrMsg = InfrastructureLanguages.Labels.Error_Server;
                ExceptionHandler.Handle(ex);
            }

            return signInResult;
        }

        public bool IsShowReCaptcha()
        {
            return CacheExtension.Get<int>(this._currentLoginFailedSessionKey) >= this._loginFailTimeToStartUsingCaptcha;
        }

        /// <summary>
        /// Get login totally failed.
        /// </summary>
        /// <returns>Return result.</returns>
        private int TotallyLoginFailed()
        {
            int count = 0;
            try
            {
                count = CacheExtension.Get<int>(_totalLoginFailedSessionKey);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return count;
        }

        /// <summary>
        /// Get login current failed.
        /// </summary>
        /// <returns>Return result.</returns>
        private int CurrentLoginFailed()
        {
            int count = 0;
            try
            {
                count = CacheExtension.Get<int>(_currentLoginFailedSessionKey);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return count;
        }

        /// <summary>
        /// Get login failed time.
        /// </summary>
        /// <returns>Return result.</returns>
        private DateTime LoginFailedTime()
        {
            DateTime date = DateTime.MinValue;
            try
            {
                if (CacheExtension.Get(_latestLoginFailSessionKey) != null)
                {
                    date = Converter.ToDateTime(CacheExtension.Get(_latestLoginFailSessionKey));
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return date;
        }

        private void SetLoginFailedTime()
        {
            try
            {
                CacheExtension.Set(_latestLoginFailSessionKey, DateTime.Now);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }
        }

        /// <summary>
        /// Reset login failed.
        /// </summary>
        private void ResetLoginFailed()
        {
            try
            {
                CacheExtension.Remove(_totalLoginFailedSessionKey);
                CacheExtension.Remove(_currentLoginFailedSessionKey);
                CacheExtension.Remove(_latestLoginFailSessionKey);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }
        }

        private void ResetCurrentLoginFailed()
        {
            try
            {
                CacheExtension.Remove(_currentLoginFailedSessionKey);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }
        }

        /// <summary>
        /// Increase Login Failed Count.
        /// </summary>
        private int IncreaseLoginFailedCount()
        {
            try
            {
                var loginFailCount = TotallyLoginFailed() + 1;
                CacheExtension.Set(_totalLoginFailedSessionKey, loginFailCount);
                var currentLoginFailCount = CurrentLoginFailed() + 1;
                CacheExtension.Set(_currentLoginFailedSessionKey, currentLoginFailCount);
                return loginFailCount;
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
                return 0;
            }
        }

        /// <summary>
        /// Last Login Username.
        /// </summary>
        /// <returns>Return result.</returns>
        private string LastLoginUserName()
        {
            try
            {
                return CacheExtension.Get<string>(this._lastLoginUserNameSessionKey);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return string.Empty;
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

            this.SetLoginFailedTime();
            var loginFailedCount = IncreaseLoginFailedCount();
            if (loginFailedCount > _maxTimesToAllowLogInFail)
            {
                var ipLockedError = string.Format(Labels.iploginfailedmultipletimeserror, signInResult.Ip, _maxTimesToAllowLogInFail, _blockLoginMinute);
                signInResult.ErrCode = SignInErrorContants.ErrorInvalidLogin;
                signInResult.ErrMsg = ipLockedError;
            }
            else
            {
                signInResult.ErrCode = verifySignInResult.ErrorCode;
                signInResult.ErrMsg = signInResult.ErrMsg = verifySignInResult.ErrorMessage;
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
            ResetLoginFailed();

            var encryptPassword = AesExtension.Encrypt(password, AppSession.EncryptCode);
            var crossCookieValue = EncryptExtension.CfsEnCode((verifySignInResult.CustId + verifySignInResult.LoginTime.Minute).ToString());

            string url;
            var robotResult = robotDetector.IsRobot(httpContext.Request);

            if (robotResult == BotFlagged.IsClean)
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
            var request = httpContext.Request;
            var filterIdentity = $"{username.Trim().ToLowerInvariant()}_{request.GetClientIp()}";
#pragma warning disable S4462 // Calls to "async" methods should not be blocking
            var result = netThrottler.ThrottleAsync(
                async (success) =>
                {
                    var requestInfo = DeviceHelper.GetRequestInfo(request, browserSize);
                    requestInfo.Add("sitegroup", UrlHelper.SiteGroupSetting.SiteGroup);
                    var verifySignInResult = _signInRepository.VerifyDownlineLogin(username, password, requestInfo);
                    if (verifySignInResult.ErrorCode == 0)
                    {
                        UpdateResultAfterLoginSuccessfully(password, signInResult, verifySignInResult, deviceId, browserSize);
                        if (whitelistSuperIds.Contains(verifySignInResult.SuperId))
                        {
                            return ThrottleResult.Allowed;
                        }

                        return await success(ThrottleResult.Allowed);
                    }
                    else
                    {
                        UpdateResultAfterLoginFail(signInResult, verifySignInResult);
                        return ThrottleResult.Allowed;
                    }
                },
                () => ThrottleResult.Blocked,
                filterIdentity,
                loginPolicy
                ).Result;
#pragma warning restore S4462 // Calls to "async" methods should not be blocking

            if (result == ThrottleResult.Blocked)
            {
                signInResult.ErrCode = SignInErrorContants.Error2;
                signInResult.ErrMsg = string.Format(Labels.LoginTooOftenBlockedMessage, username);
            }
        }
    }
}