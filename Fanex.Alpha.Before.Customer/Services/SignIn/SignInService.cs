// -----------------------------------------------------------------------
// <copyright file="SignInService.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Alpha.Before.Customer.Services.SignIn
{
    using CasinoLibrary.Model.UserException;
    using Cryptography.Aes;
    using Fanex.Alpha.Before.Common.Configurations;
    using Fanex.Alpha.Before.Common.Constants;
    using Fanex.Alpha.Before.Common.Extensions;
    using Fanex.Alpha.Before.Common.Helpers;
    using Fanex.Alpha.Before.Customer.Configurations;
    using Fanex.Alpha.Before.Customer.Constants;
    using Fanex.Alpha.Before.Customer.Models;
    using Fanex.Alpha.Before.Customer.Repositories.SignIn;
    using Fanex.Alpha.Before.Infrastructure.Constants;
    using Fanex.Alpha.Before.Infrastructure.Extensions;
    using Fanex.Common;
    using Logging.Extensions.Web;
    using RSACryptoPad;
    using System;
    using System.Diagnostics;
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

        public SignInService() :
            this(null)
        {
        }

        public SignInService(SignInRepository signInRepository)
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
            this._baseDomain = currentLoginUrl.Remove(0, currentLoginUrl.IndexOf('.') + 1);
            this._baseDomain = this._baseDomain.Remove(this._baseDomain.Length - 1, 1);
            var alphaCommonSettings = AlphaCommonSection.Instance;
            if (alphaCommonSettings != null)
            {
                this._maxTimesToAllowLogInFail = alphaCommonSettings.MaxTimesToAllowLoginFail;
                this._blockLoginMinute = alphaCommonSettings.BlockLoginMinutes;
                this._loginFailTimeToStartUsingCaptcha = alphaCommonSettings.LoginFailTimeToStartUsingCaptcha;
                this._notInteractionTimeInMinutes = alphaCommonSettings.NoInteractionTimeInMinutes;
                this._passwordMaxFailTimeForOneUser = alphaCommonSettings.PasswordMaxFailTimeForOneUser;
            }
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
                AppSession.CookieCode = EncryptExtension.RandomNumberString(6);
                var randomCookieTicket = EncryptExtension.CfsEnCode(AppSession.CookieCode);

                try
                {
                    //// Set cookie with custid id
                    HttpCookie cookie = new HttpCookie("__utms", randomCookieTicket);
                    cookie.Domain = this._baseDomain;
                    cookie.HttpOnly = true;
                    cookie.Expires = DateTime.Now.AddDays(1); // Max 24h
                    System.Web.HttpContext.Current.Response.SetCookie(cookie);
                }
                catch (Exception ex)
                {
                    ExceptionHandler.Handle(ex);
                }

                loginResult.Language = LanguageHelper.CheckingLanguageFromCookie();
                loginResult.UTMS = randomCookieTicket;
                loginResult.Domain = this._baseDomain;

                //// Use for encrypt pwd
                AppSession.EncryptCode = EncryptExtension.RandomNumberString(4);
                loginResult.Captcha = AppSession.EncryptCode;
            }
            catch (Exception ex)
            {
                loginResult.ErrCode = ErrorCodeConstants.ServerError;
                loginResult.ErrMsg = Infrastructure.Resources.Labels.Error_Server;
                ExceptionHandler.Handle(ex);
            }

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
                Username = username
            };

            try
            {
                AppSession.EncryptCode = code;

                //// Login Failed Count > 15
                if (this.TotallyLoginFailed() > _maxTimesToAllowLogInFail)
                { //// After 15'
                    if (this.LoginFailedTime().AddMinutes(_blockLoginMinute).CompareTo(DateTime.Now) > 0)
                    {
                        this.SetLoginFailedTime();
                        var ipLockedError = string.Format(LoginLanguages.Labels.iploginfailedmultipletimeserror, _maxTimesToAllowLogInFail, _blockLoginMinute);
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

                        signInResult.ErrCode = -2; ////no need to write log in fps and redirect to login page.
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

                AssignSettingLanguage(language);
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
                    this.IncreaseLoginFailedCount();
                    signInResult.ErrCode = SignInErrorContants.ErrorInvalidCaptcha;
                    signInResult.ErrMsg = LoginLanguages.Labels.errInvalidCaptcha;
                    return signInResult;
                }

                try
                {
                    password = AesExtension.Decrypt(password, AppSession.EncryptCode);
                }
                catch (Exception ex)
                {
                    ExceptionHandler.Handle(ex);
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
        /// Close RNG Account.
        /// </summary>
        /// <param name="custId">Customer ID.</param>
        private static void CloseRnGAccount(int custId)
        {
            try
            {
                CasinoLibrary.SBAgent.Customer rngCustomer = new CasinoLibrary.SBAgent.Customer();
                // if (rngCustomer.CheckCustSetting(custId))
                {
                    rngCustomer.UpdateCustClosedWrongPwd(custId, true, IPHelper.ClientIP());
                }
            }
            catch (ErrorException ex)
            {
                ExceptionHandler.Handle(new Exception(ex.Message, ex), TraceEventType.Information);
            }
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
        private void IncreaseLoginFailedCount()
        {
            try
            {
                var loginFailCount = TotallyLoginFailed() + 1;
                CacheExtension.Set(_totalLoginFailedSessionKey, loginFailCount);
                var currentLoginFailCount = CurrentLoginFailed() + 1;
                CacheExtension.Set(_currentLoginFailedSessionKey, currentLoginFailCount);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
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

        /// <summary>
        /// Setting language that user's choosing.
        /// </summary>
        /// <param name="language">The currently language.</param>
        private static void AssignSettingLanguage(string language)
        {
            AppSession.Language = LanguageHelper.IsValidLanguage(language) ? language : LanguageHelper.DefaultLanguage;

            LanguageHelper.SetLanguage(AppSession.Language);
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
            // Closed by wrong pwd 5 times, need to be synchronized to Casino DB
            if ((verifySignInResult.ErrorCode == SignInErrorContants.Error109 || verifySignInResult.ErrorCode == SignInErrorContants.Error108) && verifySignInResult.SubAccId == 0)
            {
                CloseRnGAccount(verifySignInResult.CustId);
            }

            signInResult.IsChangedPassword = verifySignInResult.LastTimeToChangePassword != DateTime.MinValue;
            if (signInResult.IsChangedPassword)
            {
                signInResult.ChangedPasswordTime = string.Format(LoginLanguages.Labels.lblPasswordChanged, verifySignInResult.LastTimeToChangePassword);
            }

            this.SetLoginFailedTime();
            this.IncreaseLoginFailedCount();

            if (this.TotallyLoginFailed() > _maxTimesToAllowLogInFail)
            {
                var ipLockedError = string.Format(LoginLanguages.Labels.iploginfailedmultipletimeserror, _maxTimesToAllowLogInFail, _blockLoginMinute);
                signInResult.ErrCode = SignInErrorContants.ErrorInvalidLogin;
                signInResult.ErrMsg = ipLockedError;
            }
            else
            {
                signInResult.ErrCode = verifySignInResult.ErrorCode;
                signInResult.ErrMsg = signInResult.ErrMsg = verifySignInResult.ErrorMessage;
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
            this.ResetLoginFailed();

            bool numberCrossCookie = System.Web.HttpContext.Current.Request.Cookies.Get("__utms") == null
                || System.Web.HttpContext.Current.Request.Cookies.Get("__utms").Value == AppSession.CookieCode;
            string crossCookieValue = EncryptExtension.CfsEnCode((verifySignInResult.CustId + verifySignInResult.LoginTime.Minute).ToString());
            int keySize = 1024;

            var path = System.Web.HttpContext.Current.Server.MapPath("~/IbcKeys/ibcpkey.snk");
            var parameter = string.Format(
                "{0}&{1}&{2}&{3}&{4}&{5}&__utms&{6}&{7}&{8}&{9}&{10}",
                verifySignInResult.CustId,
                verifySignInResult.SubAccId,
                verifySignInResult.LoginTime.Ticks,
                AesExtension.Encrypt(password, AppSession.EncryptCode),
                verifySignInResult.Nickname,
                verifySignInResult.IsNickname,
                AppSession.Language,
                numberCrossCookie,
                deviceId,
                browserSize,
                AppSession.EncryptCode);

            parameter = new EncryptionThread().EncryptString(parameter, keySize, path);

            string url = HttpContext.Current.Session["UMToken"] == null
                ? string.Format("{0}{1}?params={2}", UrlHelper.SiteGroupSetting.AfterUrl, CustomerAppSetting.AfterLoginAuthorizationPage, parameter)
                : string.Format(
                    "{0}{1}?UMToken={2}&params={3}",
                    UrlHelper.SiteGroupSetting.AfterUrl,
                    CustomerAppSetting.AfterLoginAuthorizationPage,
                    HttpContext.Current.Session["UMToken"],
                    parameter);

            signInResult.ErrMsg = LoginLanguages.Labels.lblLoginSuccessful;
            signInResult.AfterLoginUrl = url;
            signInResult.UTMS = crossCookieValue;
            signInResult.Domain = this._baseDomain;
            signInResult.ErrCode = -1;
            signInResult.Username = verifySignInResult.SubAccId == 0 ? verifySignInResult.Username : signInResult.Username;

            try
            {
                // Set cookie with custid id and seed
                HttpCookie cookie = new HttpCookie("__utms", crossCookieValue);
                cookie.Domain = this._baseDomain;
                cookie.Expires = DateTime.Now.AddDays(1); // Max 24h
                cookie.HttpOnly = true;
                System.Web.HttpContext.Current.Response.SetCookie(cookie);
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
            var requestInfo = DeviceHelper.GetRequestInfo(System.Web.HttpContext.Current.Request, browserSize);
            requestInfo.Add("sitegroup", UrlHelper.SiteGroupSetting.SiteGroup);

            var verifySignInResult = _signInRepository.VerifyDownlineLogin(username, password, requestInfo);
            if (verifySignInResult.ErrorCode == 0)
            {
                UpdateResultAfterLoginSuccessfully(password, signInResult, verifySignInResult, deviceId, browserSize);
            }
            else
            {
                UpdateResultAfterLoginFail(signInResult, verifySignInResult);
            }
        }
    }
}