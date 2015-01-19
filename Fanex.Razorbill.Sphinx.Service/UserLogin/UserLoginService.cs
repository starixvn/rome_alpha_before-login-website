// -----------------------------------------------------------------------
// <copyright file="UserLoginService.cs" company="Nexcel Solution">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Razorbill.Sphinx.Service.UserLogin
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Globalization;
    using System.Linq;
    using System.Web;
    using Fanex.Age.Utility.CaptchaUtil;
    using Fanex.Razorbill.Sphinx.Core.Configurations;
    using Fanex.Razorbill.Sphinx.Core.Constants;
    using Fanex.Razorbill.Sphinx.Core.Extensions;
    using Fanex.Razorbill.Sphinx.Resources.Resources;
    using Fanex.Razorbill.Sphinx.Service.UserLogin.Models;
    using RSACryptoPad;
    using Sunplus.Agent.Model.BL.Definitions;
    using Sunplus.Agent.Model.DL.Authorization;
    using Sunplus.Agent.Model.Services;
    using Sunplus.Agent.Utilities;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class UserLoginService
    {
        private const int MAXTIMESTOALLOWLOGINFAIL = 15;
        private const int BLOCKLOGINMINUTE = 15;
        private string _baseDomain = string.Empty;
        private string _loginTheme = string.Empty;
        private string _totalLoginFailedSessionKey = string.Format(CommonConstants.LOGINFAILEDCOUNT, IPExtension.CustomerIp());
        private string _latestLoginFailSessionKey = string.Format(CommonConstants.LOGINFAILEDTIME, IPExtension.CustomerIp());

        public UserLoginService()
        {
            ////setting base domain
            this._baseDomain = WebSettings.BaseDomain;
            string currentDomain = AppSettings.BeforeLoginUrl;
            //// Not include dot
            this._baseDomain = currentDomain.Remove(0, currentDomain.IndexOf('.') + 1);
            this._baseDomain = this._baseDomain.Remove(this._baseDomain.Length - 1, 1);       

            //// setting login template
            this._loginTheme = WebSettings.DefaultLoginTemplate;
            var startEventDate = Convert.ToDateTime(ConfigurationManager.AppSettings["SpecialEventStart"]);
            var endEventDate = Convert.ToDateTime(ConfigurationManager.AppSettings["SpecialEventEnd"]);
            var currentDate = DateTime.Now;

            if (currentDate >= startEventDate && currentDate <= endEventDate)
            {
                this._loginTheme = WebSettings.EventLoginTemplate;
            }
        }

        protected string SiteGroup
        {
            get
            {
                string host = System.Web.HttpContext.Current.Request.Headers["Host"];
                var domainGroupSection = DomainGroupSection.Instance.DomainGroup;
                var defaultGroup = "Others";

                if (domainGroupSection == null || domainGroupSection.Count == 0)
                {
                    return defaultGroup;
                }

                var getDomainGroupActive = from m in domainGroupSection.Cast<DomainGroupElement>().Where(m => m.Active) select m;

                if (!getDomainGroupActive.Any())
                {
                    return defaultGroup;
                }

                foreach (DomainGroupElement domainGroupElement in getDomainGroupActive)
                {
                    if (string.IsNullOrWhiteSpace(domainGroupElement.Domains))
                    {
                        return defaultGroup;
                    }

                    var domainList = new List<string>(domainGroupElement.Domains.Split(','));

                    if (domainList.Contains(host))
                    {
                        return domainGroupElement.Name;
                    }
                }

                return defaultGroup;
            }
        }

        /// <summary>
        /// Sign in progress.
        /// </summary>
        /// <returns>Return result.</returns>
        public LoginResult SignIn()
        {
            var language = CheckingLanguageFromCookie();
            var loginResult = new LoginResult()
            {
                LoginTemplate = _loginTheme,
                Username = string.Empty
            };

            try
            {
                //// Check cross cookie compatibility
                AppSession.Captcha3 = Encryption.RandomNumberString(6);
                string randomTicket = Encryption.CfsEnCode(AppSession.Captcha3);

                try
                {
                    //// Set cookie with custid id
                    HttpCookie cookie = new HttpCookie("__utms", randomTicket);
                    cookie.Domain = this._baseDomain;
                    cookie.Expires = DateTime.Now.AddDays(1); // Max 24h
                    System.Web.HttpContext.Current.Response.SetCookie(cookie);
                }
                catch (Exception ex)
                {
                    LogExtension.ExceptionHandler(ex);
                }

                loginResult.Language = language;
                loginResult.Ip = IPExtension.CustomerIp();
                loginResult.UTMS = randomTicket;
                loginResult.Domain = this._baseDomain;

                //// Use for encrypt pwd
                AppSession.Captcha2 = Encryption.RandomNumberString(4);
                loginResult.Captcha = AppSession.Captcha2;
            }
            catch (Exception ex)
            {
                loginResult.ErrCode = ErrorContants.ErrorDefault;
                loginResult.ErrMsg = Language.HttpError_Default;
                LogExtension.ExceptionHandler(ex);
            }

            return loginResult;
        }

        /// <summary>
        /// Sign in page.
        /// </summary>
        /// <param name="username">User name.</param>
        /// <param name="password">User password.</param>
        /// <param name="language">User language.</param>
        /// <param name="captcha">Captcha value.</param>
        /// <param name="code">Encrypt code.</param>
        /// <returns>Return result.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public LoginResult SignIn(string username, string password, string language, string captcha, string code)
        {
            var loginResult = new LoginResult()
            {
                Ip = IPExtension.CustomerIp(),
                LoginTemplate = _loginTheme,
                Username = username
            };

            try
            {
                AppSession.Captcha2 = code;
                ////Login Failed Count > 15
                if (this.GetLoginFailedCount() > MAXTIMESTOALLOWLOGINFAIL)
                {
                    ////After 15'
                    if (this.GetLoginFailedTime().AddMinutes(BLOCKLOGINMINUTE).CompareTo(DateTime.Now) > 0)
                    {
                        this.SetLoginFailedTime();
                        var ipLockedError = string.Format(Language.iploginfailedmultipletimeserror, MAXTIMESTOALLOWLOGINFAIL, BLOCKLOGINMINUTE);

                        loginResult.ErrCode = ErrorContants.ErrorInvalidLogin;
                        loginResult.ErrMsg = ipLockedError;

                        return loginResult;
                    }
                    else
                    {
                        this.ResetLoginFailed();
                    }
                }

                if (WebSettings.IsValidLanguage(language))
                {
                    AppSession.Language = language;
                }
                else
                {
                    AppSession.Language = WebSettings.DefaultLanguage;
                }

                this.SetLanguage(AppSession.Language);

                if (AppSession.Captcha2 == null)
                {
                    loginResult.ErrCode = ErrorContants.ErrorInvalidCaptcha;
                    loginResult.ErrMsg = Language.errInvalidCaptcha;

                    return loginResult;
                }
                else
                {
                    loginResult.Captcha = AppSession.Captcha2;
                }

                if (captcha != AppSession.CaptchaCode)
                {
                    this.IncreaseLoginFailedCount();
                    loginResult.ErrCode = ErrorContants.ErrorInvalidCaptcha;
                    loginResult.ErrMsg = Language.errInvalidCaptcha;

                    return loginResult;
                }

                // Create new code
                AppSession.CaptchaCode = Encryption.RandomNumberString(4).ToUpperInvariant();

                int errCode = -1;
                string errMsg = null;
                int custId = 0, subAccId = 0;
                bool isNickname = false;
                string nickName = string.Empty;
                DateTime loginTime = DateTime.Now;
                var lastChangedPwd = DateTime.MinValue;

                try
                {
                    password = Encryption.RC4(AppSession.Captcha2, Encryption.HexDecode(password));
                }
                catch (Exception ex)
                {
                    LogExtension.ExceptionHandler(ex);
                    loginResult.ErrCode = ErrorContants.ErrorDefault;
                    loginResult.ErrMsg = string.Empty;

                    return loginResult;
                }

                // Get Browser
                Hashtable requestInfo = DeviceService.GetRequestInfo(System.Web.HttpContext.Current.Request);
                requestInfo.Add("sitegroup", SiteGroup);
                VerifyDownlineLogin(
                    username,
                    password,
                    ref errCode,
                    ref errMsg,
                    ref custId,
                    ref subAccId,
                    ref isNickname,
                    ref nickName,
                    ref loginTime,
                    ref lastChangedPwd,
                    requestInfo);

                if (errCode == 0)
                {
                    UpdateResultAfterLoginSuccessfully(password, loginResult, custId, subAccId, isNickname, nickName, loginTime);
                }
                else
                {
                    UpdateResultAfterLoginFail(loginResult, errCode, errMsg, custId, subAccId, lastChangedPwd);
                }
            }
            catch (Exception ex)
            {
                loginResult.ErrCode = ErrorContants.ErrorDefault;
                loginResult.ErrMsg = Language.HttpError_Default;
                LogExtension.ExceptionHandler(ex);
            }

            return loginResult;
        }
        
        /// <summary>
        /// Get new captcha.
        /// </summary>
        /// <returns>Return result.</returns>
        public Graphics GetNewCaptcha() 
        {
            Graphics graphic = null;
            try
            {
                System.Web.HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                System.Web.HttpContext.Current.Response.Cache.SetNoStore();

                var captchaUtil = new CaptchaImage(IPExtension.CustomerIp(), AppSession.Username, CaptchaType.Text);
                var code = string.Empty;
                var value = string.Empty;
                Image bitmap = captchaUtil.Generate(out code, out value);
                AppSession.CaptchaCode = value;
                ////AppSessions.CaptchaType = captchaUtil.Choice;

                //// Set the image.
                //// Change the response headers to output a JPEG image.
                System.Web.HttpContext.Current.Response.Clear();

                //// Write the image to the response stream in JPEG format.
                bitmap.Save(System.Web.HttpContext.Current.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg);

                ////Create new image
                graphic = Graphics.FromImage(bitmap);
            }
            catch (Exception ex)
            {
                LogExtension.ExceptionHandler(ex);
            }
            
            return graphic;
        }

        #region private functions
        protected void SetLanguage(string language)
        {
            System.Globalization.CultureInfo def = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
            Language.Culture = new CultureInfo(language);
        }

        /// <summary>
        /// Get login totally failed.
        /// </summary>
        /// <returns>Return result.</returns>
        protected int GetLoginFailedCount()
        {
            int count = 0;
            try
            {
                if (CacheExtension.Get(_totalLoginFailedSessionKey) is int)
                {
                    count = (int)CacheExtension.Get(_totalLoginFailedSessionKey);
                }
            }
            catch (Exception e)
            {
                Logs.Write(e);
            }

            return count;
        }

        protected void IncreaseLoginFailedCount()
        {
            try
            {
                CacheExtension.Set(_totalLoginFailedSessionKey, this.GetLoginFailedCount() + 1);
            }
            catch (Exception e)
            {
                Logs.Write(e);
            }
        }

        /// <summary>
        /// Reset login failed.
        /// </summary>
        protected void ResetLoginFailed()
        {
            try
            {
                CacheExtension.Delete(_totalLoginFailedSessionKey);
                CacheExtension.Delete(_latestLoginFailSessionKey);
            }
            catch (Exception e)
            {
                Logs.Write(e);
            }
        }

        protected void SetLoginFailedTime()
        {
            try
            {
                CacheExtension.Set(_latestLoginFailSessionKey, DateTime.Now);
            }
            catch (Exception e)
            {
                Logs.Write(e);
            }
        }

        /// <summary>
        /// Get login failed time.
        /// </summary>
        /// <returns>Return result.</returns>
        protected DateTime GetLoginFailedTime()
        {
            DateTime date = DateTime.MinValue;
            try
            {
                if (CacheExtension.Get(_latestLoginFailSessionKey) is DateTime)
                {
                    date = (DateTime)CacheExtension.Get(_latestLoginFailSessionKey);
                }
            }
            catch (Exception e)
            {
                Logs.Write(e);
            }

            return date;
        }
        
        /// <summary>
        /// Checking language from cookie.
        /// </summary>
        /// <returns>Return result.</returns>
        private static int CheckingLanguageFromCookie()
        {
            var language = 0;
            if (System.Web.HttpContext.Current.Request.Cookies["LANGUAGE"] != null)
            {
                int.TryParse(System.Web.HttpContext.Current.Request.Cookies["LANGUAGE"].Value, out language);
            }

            if (language > 5)
            {
                language = 0;
            }

            return language;
        }
        
        /// <summary>
        /// Verify down-line login.
        /// </summary>
        /// <param name="username">User name.</param>
        /// <param name="password">User password.</param>
        /// <param name="errCode">Error code.</param>
        /// <param name="errMsg">Error message.</param>
        /// <param name="custId">Customer ID.</param>
        /// <param name="subAccId">Sub account ID.</param>
        /// <param name="isNickname">Is nick-name.</param>
        /// <param name="nickName">Nick name.</param>
        /// <param name="loginTime">Login time.</param>
        /// <param name="lastChangedPwd">Last change password.</param>
        /// <param name="requestInfo">Request information.</param>
        private static void VerifyDownlineLogin(
            string username,
            string password,
            ref int errCode,
            ref string errMsg,
            ref int custId,
            ref int subAccId,
            ref bool isNickname,
            ref string nickName,
            ref DateTime loginTime,
            ref DateTime lastChangedPwd,
            Hashtable requestInfo)
        {
            try
            {
                new AuthorizationDAO().Age_LoginVerifyDownline(
                    username.Trim(),
                    password,
                    requestInfo,
                    AppSession.LangKey,
                    out custId,
                    out subAccId,
                    out loginTime,
                    out lastChangedPwd,
                    out nickName,
                    out isNickname,
                    out errCode,
                    out errMsg);
            }
            catch (Exception ex)
            {
                errCode = ErrorContants.ErrorInvalidCode909;
                errMsg = Language.invalidLogin909;
                LogExtension.ExceptionHandler(ex);
            }
        }

        /// <summary>
        /// Update result login after login Fail.
        /// </summary>
        /// <param name="loginResult">Login result.</param>
        /// <param name="errCode">Error code.</param>
        /// <param name="errMsg">Error message.</param>
        /// <param name="custId">Customer ID.</param>
        /// <param name="subAccId">Sub account ID.</param>
        /// <param name="lastChangedPwd">Last change password.</param>
        private void UpdateResultAfterLoginFail(
            LoginResult loginResult, 
            int errCode, 
            string errMsg, 
            int custId,
            int subAccId,
            DateTime lastChangedPwd)
        {
            // Closed by wrong pwd 5 times, need to be synchronized to Casino DB
            if (errCode == ErrorContants.Error109 && subAccId == 0)
            {
                try
                {
                    CasinoLibrary.SBAgent.Customer customer = new CasinoLibrary.SBAgent.Customer();
                    customer.UpdateCustClosedWrongPwd(custId, true, IPExtension.CustomerIp());
                }
                catch (Exception ex)
                {
                    LogExtension.ExceptionHandler(ex);
                }
            }

            loginResult.IsChangedPassword = lastChangedPwd != DateTime.MinValue;
            if (loginResult.IsChangedPassword)
            {
                loginResult.ChangedPasswordTime = string.Format(Language.lblPasswordChanged, lastChangedPwd);
            }

            this.SetLoginFailedTime();
            this.IncreaseLoginFailedCount();
            if (this.GetLoginFailedCount() > MAXTIMESTOALLOWLOGINFAIL)
            {
                var ipLockedError = string.Format(Language.iploginfailedmultipletimeserror, MAXTIMESTOALLOWLOGINFAIL, BLOCKLOGINMINUTE);

                loginResult.ErrCode = 1;
                loginResult.ErrMsg = ipLockedError;
            }
            else
            {
                loginResult.ErrCode = errCode;
                loginResult.ErrMsg = errMsg;
            }
        }

        /// <summary>
        /// Update result login after login successfully.
        /// </summary>
        /// <param name="password">User password.</param>
        /// <param name="loginResult">Login result.</param>
        /// <param name="custId">Customer ID.</param>
        /// <param name="subAccId">Sub account ID.</param>
        /// <param name="isNickname">Is nick name.</param>
        /// <param name="nickName">Nick name.</param>
        /// <param name="loginTime">Login time.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void UpdateResultAfterLoginSuccessfully(
            string password, 
            LoginResult loginResult, 
            int custId, 
            int subAccId, 
            bool isNickname, 
            string nickName, 
            DateTime loginTime)
        {
            this.ResetLoginFailed();

            bool numberCrossCookie = System.Web.HttpContext.Current.Request.Cookies.Get("__utms") == null
                || System.Web.HttpContext.Current.Request.Cookies.Get("__utms").Value == AppSession.Captcha3;
            string crossCookieValue = Encryption.CfsEnCode((custId + loginTime.Minute).ToString());
            int keySize = 1024;

            string parameter = string.Empty, path = string.Empty;
            parameter = custId.ToString() + "&" ////0
                        + subAccId.ToString() + "&" ////1
                        + loginTime.Ticks + "&" ////2
                        + Encryption.HexEncode(Encryption.RC4(loginTime.Minute.ToString(), password)) + "&" ////3
                        + nickName + "&" ////4
                        + isNickname.ToString() + "&" ////5
                        + "__utms" + "&" ////6
                        + AppSession.Language + "&"////7
                        + numberCrossCookie; ////8

            path = System.Web.HttpContext.Current.Server.MapPath("~/IbcKeys/ibcpkey.snk");
            EncryptionThread encryption = new EncryptionThread();
            parameter = encryption.EncryptString(parameter, keySize, path);

            string url = AppSettings.AfterLoginUrl;
            /*if (System.Web.HttpContext.Current.Request.Cookies["ssl"] == null || System.Web.HttpContext.Current.Request.Cookies["ssl"].Value != "true")
            {
                url = url.Replace("https://", "http://");
            }*/

            url += WebSettings.AfterLoginPage;
            url += "?params=" + parameter;

            loginResult.ErrMsg = Language.lblLoginSuccessful;
            loginResult.AfterLoginUrl = url;
            loginResult.UTMS = crossCookieValue;
            loginResult.Domain = this._baseDomain;
            loginResult.ErrCode = -1;
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
        #endregion
    }
}
