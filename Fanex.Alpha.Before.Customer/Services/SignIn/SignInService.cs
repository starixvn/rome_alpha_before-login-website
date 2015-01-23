// -----------------------------------------------------------------------
// <copyright file="SignInService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Alpha.Before.Customer.Services.SignIn
{
    using System;
    using System.Collections;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Linq;
    using System.Web;
    using Fanex.Alpha.Before.Common.Configurations;
    using Fanex.Alpha.Before.Common.Constants;
    using Fanex.Alpha.Before.Common.Extensions;
    using Fanex.Alpha.Before.Common.Helpers;
    using Fanex.Alpha.Before.Common.Utility.CaptchaUtility;
    using Fanex.Alpha.Before.Customer.Configurations;
    using Fanex.Alpha.Before.Customer.Constants;
    using Fanex.Alpha.Before.Customer.Models;
    using Fanex.Alpha.Before.Customer.Repositories.SignIn;
    using Fanex.Alpha.Before.Infrastructure.Constants;
    using Fanex.Alpha.Before.Infrastructure.Extensions;
    using Fanex.Common;
    using RSACryptoPad;
    using InfrastructureLanguages = Fanex.Alpha.Before.Infrastructure.Resources;
    using LoginLanguages = Fanex.Alpha.Before.Customer.Resources;

    /// <summary>
    /// Sign in service.
    /// </summary>
    public class SignInService : ISignInService
    {
        private const int MAXTIMESTOALLOWLOGINFAIL = 15;
        private const int BLOCKLOGINMINUTE = 15;
        private string _baseDomain = string.Empty;
        private string _loginTheme = string.Empty;
        private LoginThemeSection _loginThemeSection;
        private ISignInRepository _signInRepository;
        private string _totalLoginFailedSessionKey = string.Format(SessionConstants.LOGINFAILEDCOUNT, IPHelper.ClientIP());
        private string _latestLoginFailSessionKey = string.Format(SessionConstants.LOGINFAILEDTIME, IPHelper.ClientIP());
        
        public SignInService() :
            this(null)
        {            
        }

        public SignInService(SignInRepository signInRepository)
        {
            _signInRepository = signInRepository ?? new SignInRepository();
            _loginThemeSection = LoginThemeSection.Instance;

            this._loginTheme = _loginThemeSection.DefaultTheme;
            if (DateTime.Now >= Converter.ToDateTime(_loginThemeSection.StartDateShowEventTheme) &&
                DateTime.Now <= Converter.ToDateTime(_loginThemeSection.EndDateShowEventTheme))
            {
                this._loginTheme = _loginThemeSection.EventTheme;
            }

            var currentLoginUrl = UrlHelper.SiteGroupSetting.BeforeUrl;
            this._baseDomain = currentLoginUrl.Remove(0, currentLoginUrl.IndexOf('.') + 1);
            this._baseDomain = this._baseDomain.Remove(this._baseDomain.Length - 1, 1);
        }

        /// <summary>
        /// Sign In default method.
        /// </summary>
        /// <returns>Return result.</returns>
        public SignInResult SignIn()
        {
            var loginResult = new SignInResult()
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
                    cookie.Expires = DateTime.Now.AddDays(1); // Max 24h
                    System.Web.HttpContext.Current.Response.SetCookie(cookie);
                }
                catch (Exception ex)
                {
                    LogExtension.ExceptionHandler(ex);
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
                LogExtension.ExceptionHandler(ex);
            }

            return loginResult;
        }

        /// <summary>
        /// Sign In method.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">User's password.</param>
        /// <param name="language">Current language.</param>
        /// <param name="captcha">The captcha.</param>
        /// <param name="code">The encrypt code.</param>
        /// <returns>Return login result.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public SignInResult SignIn(string username, string password, string language, string captcha, string code)
        {
            var signInResult = new SignInResult()
            {
                Ip = IPHelper.ClientIP(),
                LoginTemplate = _loginTheme,
                Username = username
            };

            try
            {
                AppSession.EncryptCode = code;
                //// Login Failed Count > 15
                if (this.TotallyLoginFailed() > MAXTIMESTOALLOWLOGINFAIL)
                { //// After 15'
                    if (this.LoginFailedTime().AddMinutes(BLOCKLOGINMINUTE).CompareTo(DateTime.Now) > 0)
                    {
                        this.SetLoginFailedTime();
                        var ipLockedError = string.Format(LoginLanguages.Labels.iploginfailedmultipletimeserror, MAXTIMESTOALLOWLOGINFAIL, BLOCKLOGINMINUTE);
                        signInResult.ErrCode = SignInErrorContants.ErrorInvalidLogin;
                        signInResult.ErrMsg = ipLockedError;

                        return signInResult;
                    }
                    else
                    {
                        this.ResetLoginFailed();
                    }
                }

                AssignSettingLanguage(language);

                if (AppSession.EncryptCode == null)
                {
                    signInResult.ErrCode = SignInErrorContants.ErrorInvalidCaptcha;
                    signInResult.ErrMsg = LoginLanguages.Labels.errInvalidCaptcha;

                    return signInResult;
                }
                else
                {
                    signInResult.Captcha = AppSession.EncryptCode;
                }

                if (captcha != AppSession.CaptchaCode)
                {
                    this.IncreaseLoginFailedCount();
                    signInResult.ErrCode = SignInErrorContants.ErrorInvalidCaptcha;
                    signInResult.ErrMsg = LoginLanguages.Labels.errInvalidCaptcha;

                    return signInResult;
                }

                //// Create new code
                AppSession.CaptchaCode = EncryptExtension.RandomNumberString(4).ToUpperInvariant();

                try
                {
                    password = EncryptExtension.RC4(AppSession.EncryptCode, EncryptExtension.HexDecode(password));
                }
                catch (Exception ex)
                {
                    LogExtension.ExceptionHandler(ex);
                    signInResult.ErrCode = ErrorCodeConstants.ServerError;
                    signInResult.ErrMsg = string.Empty;

                    return signInResult;
                }

                //// Get Browser
                Hashtable requestInfo = DeviceHelper.GetRequestInfo(System.Web.HttpContext.Current.Request);
                requestInfo.Add("sitegroup", UrlHelper.SiteGroupSetting.SiteGroup);
                var verifySignInResult = _signInRepository.VerifyDownlineLogin(username, password, requestInfo);

                if (verifySignInResult.ErrorCode == 0)
                {
                    UpdateResultAfterLoginSuccessfully(password, signInResult, verifySignInResult);
                }
                else
                {
                    UpdateResultAfterLoginFail(signInResult, verifySignInResult);
                }
            }
            catch (Exception ex)
            {
                signInResult.ErrCode = ErrorCodeConstants.ServerError;
                signInResult.ErrMsg = InfrastructureLanguages.Labels.Error_Server;
                LogExtension.ExceptionHandler(ex);
            }

            return signInResult;
        }

        /// <summary>
        /// Support for automation test tool.
        /// </summary>
        /// <param name="username">Username login.</param>
        /// <param name="password">User's password.</param>
        /// <param name="captcha">Captcha value.</param>
        /// <returns>Return result.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public SignInResult AutomationTest(string username, string password, string captcha)
        {
            var signInResult = new SignInResult()
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

                var allowWords = allowUsernameContainWords.ToUpper().Split(',');
                if (!allowIps.Split(',').Contains(IPHelper.ClientIP()) ||
                    !allowCaptcha.Equals(captcha) ||
                    !allowWords.All(s => username.ToUpper().Contains(s)))
                {
                    return signInResult;
                }         
                                
                //// Get Browser
                Hashtable requestInfo = DeviceHelper.GetRequestInfo(System.Web.HttpContext.Current.Request);
                requestInfo.Add("sitegroup", UrlHelper.SiteGroupSetting.SiteGroup);
                var verifySignInResult = _signInRepository.VerifyDownlineLogin(username, password, requestInfo);

                if (verifySignInResult.ErrorCode == 0)
                {
                    UpdateResultAfterLoginSuccessfully(password, signInResult, verifySignInResult);
                }
                else
                {
                    UpdateResultAfterLoginFail(signInResult, verifySignInResult);
                }
            }
            catch (Exception ex)
            {
                signInResult.ErrCode = ErrorCodeConstants.ServerError;
                signInResult.ErrMsg = InfrastructureLanguages.Labels.Error_Server;
                LogExtension.ExceptionHandler(ex);
            }

            return signInResult;
        }

        /// <summary>
        /// Get new captcha.
        /// </summary>
        /// <returns>Return result.</returns>
        public Graphics NewCaptcha()
        {
            Graphics graphic = null;
            try
            {
                System.Web.HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                System.Web.HttpContext.Current.Response.Cache.SetNoStore();

                var captchaUtil = new CaptchaImage(IPHelper.ClientIP(), AppSession.Username, CaptchaType.Text);
                var code = string.Empty;
                var value = string.Empty;
                Image bitmap = captchaUtil.Generate(out code, out value);
                AppSession.CaptchaCode = value;

                //// Set the image.
                //// Change the response headers to output a JPEG image.
                System.Web.HttpContext.Current.Response.Clear();

                //// Write the image to the response stream in JPEG format.
                bitmap.Save(System.Web.HttpContext.Current.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg);

                //// Create new image
                graphic = Graphics.FromImage(bitmap);
            }
            catch (Exception ex)
            {
                LogExtension.ExceptionHandler(ex);
            }

            return graphic;
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
                if (rngCustomer.CheckCustSetting(custId))
                {
                    rngCustomer.UpdateCustClosedWrongPwd(custId, true, IPHelper.ClientIP());
                }
            }
            catch (Exception ex)
            {
                LogExtension.ExceptionHandler(ex);
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
                if (CacheExtension.Get(_totalLoginFailedSessionKey) is int)
                {
                    count = (int)CacheExtension.Get(_totalLoginFailedSessionKey);
                }
            }
            catch (Exception ex)
            {
                LogExtension.ExceptionHandler(ex);
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
                if (CacheExtension.Get(_latestLoginFailSessionKey) is DateTime)
                {
                    date = (DateTime)CacheExtension.Get(_latestLoginFailSessionKey);
                }
            }
            catch (Exception ex)
            {
                LogExtension.ExceptionHandler(ex);
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
                LogExtension.ExceptionHandler(ex);
            }
        }

        /// <summary>
        /// Reset login failed.
        /// </summary>
        private void ResetLoginFailed()
        {
            try
            {
                CacheExtension.Delete(_totalLoginFailedSessionKey);
                CacheExtension.Delete(_latestLoginFailSessionKey);
            }
            catch (Exception ex)
            {
                LogExtension.ExceptionHandler(ex);
            }
        }

        private void IncreaseLoginFailedCount()
        {
            try
            {
                CacheExtension.Set(_totalLoginFailedSessionKey, this.TotallyLoginFailed() + 1);
            }
            catch (Exception ex)
            {
                LogExtension.ExceptionHandler(ex);
            }
        }
        
        /// <summary>
        /// Setting language that user's choosing.
        /// </summary>
        /// <param name="language">The currently language.</param>
        private void AssignSettingLanguage(string language)
        {
            if (LanguageHelper.IsValidLanguage(language))
            {
                AppSession.Language = language;
            }
            else
            {
                AppSession.Language = LanguageHelper.DefaultLanguage;
            }

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
            if (verifySignInResult.ErrorCode == SignInErrorContants.Error109 && verifySignInResult.SubAccId == 0)
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

            if (this.TotallyLoginFailed() > MAXTIMESTOALLOWLOGINFAIL)
            {
                var ipLockedError = string.Format(LoginLanguages.Labels.iploginfailedmultipletimeserror, MAXTIMESTOALLOWLOGINFAIL, BLOCKLOGINMINUTE);
                signInResult.ErrCode = SignInErrorContants.ErrorInvalidLogin;
                signInResult.ErrMsg = ipLockedError;
            }
            else
            {
                signInResult.ErrCode = verifySignInResult.ErrorCode;
                signInResult.ErrMsg = verifySignInResult.ErrorMessage;
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
            VerifySignInResult verifySignInResult)
        {
            this.ResetLoginFailed();

            bool numberCrossCookie = System.Web.HttpContext.Current.Request.Cookies.Get("__utms") == null
                || System.Web.HttpContext.Current.Request.Cookies.Get("__utms").Value == AppSession.CookieCode;
            string crossCookieValue = EncryptExtension.CfsEnCode((verifySignInResult.CustId + verifySignInResult.LoginTime.Minute).ToString());
            int keySize = 1024;

            var path = System.Web.HttpContext.Current.Server.MapPath("~/IbcKeys/ibcpkey.snk");
            var parameter = string.Format(
                "{0}&{1}&{2}&{3}&{4}&{5}&__utms&{6}&{7}", 
                verifySignInResult.CustId,
                verifySignInResult.SubAccId,
                verifySignInResult.LoginTime.Ticks,
                EncryptExtension.HexEncode(EncryptExtension.RC4(verifySignInResult.LoginTime.Minute.ToString(), password)),
                verifySignInResult.Nickname,
                verifySignInResult.IsNickname,
                AppSession.Language,
                numberCrossCookie);

            parameter = new EncryptionThread().EncryptString(parameter, keySize, path);

            string url = string.Format(
                "{0}{1}?params={2}", 
                UrlHelper.SiteGroupSetting.AfterUrl, 
                CustomerAppSetting.AfterLoginAuthorizationPage, 
                parameter);

            signInResult.ErrMsg = LoginLanguages.Labels.lblLoginSuccessful;
            signInResult.AfterLoginUrl = url;
            signInResult.UTMS = crossCookieValue;
            signInResult.Domain = this._baseDomain;
            signInResult.ErrCode = -1;
            try
            {
                // Set cookie with custid id and seed
                HttpCookie cookie = new HttpCookie("__utms", crossCookieValue);
                cookie.Domain = this._baseDomain;
                cookie.Expires = DateTime.Now.AddDays(1); // Max 24h
                System.Web.HttpContext.Current.Response.SetCookie(cookie);
            }
            catch (Exception ex)
            {
                LogExtension.ExceptionHandler(ex);
            }
        }
    }
}