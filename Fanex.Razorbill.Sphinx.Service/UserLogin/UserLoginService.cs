// -----------------------------------------------------------------------
// <copyright file="UserLoginService.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.Configuration;
using System.Globalization;
using Fanex.Razorbill.Sphinx.Resources;
using Fanex.Razorbill.Sphinx.Resources.Resources;

namespace Fanex.Razorbill.Sphinx.Service.UserLogin
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Web;
    using Fanex.Age.Utility.CaptchaUtil;
    using Fanex.Razorbill.Sphinx.Core.Configurations;
    using Fanex.Razorbill.Sphinx.Core.Constants;
    using Fanex.Razorbill.Sphinx.Core.Extensions;
    
    using Fanex.Razorbill.Sphinx.Service.UserLogin.Models;
    using RSACryptoPad;
    using Sunplus.Agent.Model.BL.Definitions;
    using Sunplus.Agent.Model.DL.Authorization;
    using Sunplus.Agent.Model.Services;
    using Sunplus.Agent.Utilities;
    using System.Linq;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class UserLoginService
    {
        private string baseDomain = string.Empty;
        private string loginTheme = string.Empty;

        public UserLoginService()
        {
            #region setting base domain
            this.baseDomain = WebSettings.BaseDomain;
            string currentDomain = AppSettings.BeforeLoginUrl;
            //// Not include dot
            this.baseDomain = currentDomain.Remove(0, currentDomain.IndexOf('.') + 1);
            this.baseDomain = this.baseDomain.Remove(this.baseDomain.Length - 1, 1);
            #endregion            

            #region setting login template
            this.loginTheme = WebSettings.DefaultLoginTemplate;
            var startEventDate = Convert.ToDateTime(ConfigurationManager.AppSettings["SpecialEventStart"]);
            var endEventDate = Convert.ToDateTime(ConfigurationManager.AppSettings["SpecialEventEnd"]);
            var currentDate = DateTime.Now;

            if (currentDate >= startEventDate && currentDate <= endEventDate)
            {
                this.loginTheme = WebSettings.EventLoginTemplate;
            }
            #endregion
        }
        
        public LoginResult SignIn()
        {           

            #region set language
            var language = 0;
            if (System.Web.HttpContext.Current.Request.Cookies["LANGUAGE"] != null)
            {
                int.TryParse(System.Web.HttpContext.Current.Request.Cookies["LANGUAGE"].Value, out language);
            }

            if (language > 5)
            {
                language = 0;
            }
            #endregion

            var loginResult = new LoginResult(){
                LoginTemplate = loginTheme,
                Username = string.Empty
            };
            try
            {
                #region Check cross cookie compatibility
                AppSession.Captcha3 = Encryption.RandomNumberString(6);
                string randomTicket = Encryption.CfsEnCode(AppSession.Captcha3);

                try
                {
                    // Set cookie with custid id
                    HttpCookie cookie = new HttpCookie("__utms", randomTicket);
                    cookie.Domain = this.baseDomain;
                    cookie.Expires = DateTime.Now.AddDays(1); // Max 24h
                    System.Web.HttpContext.Current.Response.SetCookie(cookie);
                }
                catch(Exception ex)
                {
                    LogExtension.ExceptionHandler(ex);
                }
                #endregion

                loginResult.Language = language;
                loginResult.Ip = Sunplus.Agent.Model.Common.AgentIPFormat.GetClientIP();
                loginResult.UTMS = randomTicket;
                loginResult.Domain = this.baseDomain;

                #region Use for encrypt pwd
                AppSession.Captcha2 = Encryption.RandomNumberString(4);
                loginResult.Captcha = AppSession.Captcha2;
                #endregion
                
            }
            catch (Exception ex)
            {
                loginResult.ErrCode = ErrorContants.ErrorDefault;
                loginResult.ErrMsg = Language.HttpError_Default;
                LogExtension.ExceptionHandler(ex);
            }
            return loginResult;
        }

        public LoginResult SignIn(string username, string password, string language, string captcha, string code)
        {
            var loginResult = new LoginResult()
            {
                Ip = Sunplus.Agent.Model.Common.AgentIPFormat.GetClientIP(),
                LoginTemplate = loginTheme,
                Username = username
            };
            try
            {

                AppSession.Captcha2 = code;

                ////Login Failed Count > 5
                if (this.GetLoginFailedCount() > 15)
                {
                    ////After 5'
                    if (this.GetLoginFailedTime().AddMinutes(15).CompareTo(DateTime.Now) > 0)
                    {
                        this.SetLoginFailedTime();
                        loginResult.ErrCode = ErrorContants.ErrorInvalidLogin;
                        loginResult.ErrMsg = Language.invalidLogin;

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

                var au = new AuthorizationDAO();

                try
                {
                    au.Age_LoginVerifyDownline(
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

                if (errCode == 0)
                {
                    this.ResetLoginFailed();

                    bool noCrossCookie = System.Web.HttpContext.Current.Request.Cookies.Get("__utms") == null
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
                                + noCrossCookie; ////8

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
                    loginResult.Domain = this.baseDomain;
                    loginResult.ErrCode = -1;
                    try
                    {
                        // Set cookie with custid id and seed
                        HttpCookie cookie = new HttpCookie("__utms", crossCookieValue);
                        cookie.Domain = this.baseDomain;
                        cookie.Expires = DateTime.Now.AddDays(1); // Max 24h
                        System.Web.HttpContext.Current.Response.SetCookie(cookie);
                    }
                    catch(Exception ex)
                    {
                        LogExtension.ExceptionHandler(ex);
                    }
                }
                else
                {
                    // Closed by wrong pwd 5 times, need to be synchronized to Casino DB
                    if (errCode == ErrorContants.Error180)
                    {
                        try
                        {
                            CasinoLibrary.SBAgent.Customer customer = new CasinoLibrary.SBAgent.Customer();
                            customer.UpdateCustClosedWrongPwd(custId, true, Sunplus.Agent.Model.Common.AgentIPFormat.GetClientIP());
                        }
                        catch(Exception ex)
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
                    if (this.GetLoginFailedCount() > 15)
                    {
                        loginResult.ErrCode = 1;
                        loginResult.ErrMsg = Language.invalidLogin;
                    }
                    else
                    {
                        loginResult.ErrCode = errCode;
                        loginResult.ErrMsg = errMsg;
                    }
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

        public Graphics GetNewCaptcha() 
        {
            Graphics graphic = null;
            try
            {
                System.Web.HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                System.Web.HttpContext.Current.Response.Cache.SetNoStore();

                var ip = System.Web.HttpContext.Current.Request.UserHostAddress;
                var captchaUtil = new CaptchaImage(ip, AppSession.Username, CaptchaType.Text);
                var code = string.Empty;
                var value = string.Empty;
                Image bitmap = captchaUtil.Generate(out code, out value);
                AppSession.CaptchaCode = value;
                //AppSessions.CaptchaType = captchaUtil.Choice;

                // Set the image.
                // Change the response headers to output a JPEG image.
                System.Web.HttpContext.Current.Response.Clear();

                // Write the image to the response stream in JPEG format.
                bitmap.Save(System.Web.HttpContext.Current.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg);

                //Create new image
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

            /*System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(language);
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture(language);*/

            //System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat = def.DateTimeFormat;

            Language.Culture = new CultureInfo(language);
        }

        protected int GetLoginFailedCount()
        {
            int count = 0;
            try
            {
                if (CacheExtension.Get(string.Format(CommonConstants.LOGINFAILEDCOUNT, System.Web.HttpContext.Current.Request.UserHostAddress)) is int)
                {
                    count = (int)CacheExtension.Get(string.Format(CommonConstants.LOGINFAILEDCOUNT, System.Web.HttpContext.Current.Request.UserHostAddress));
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
                CacheExtension.Set(string.Format(CommonConstants.LOGINFAILEDCOUNT, System.Web.HttpContext.Current.Request.UserHostAddress), this.GetLoginFailedCount() + 1);
            }
            catch (Exception e)
            {
                Logs.Write(e);
            }
        }

        protected void ResetLoginFailed()
        {
            try
            {
                CacheExtension.Delete(string.Format(CommonConstants.LOGINFAILEDCOUNT, System.Web.HttpContext.Current.Request.UserHostAddress));
                CacheExtension.Delete(string.Format(CommonConstants.LOGINFAILEDTIME, System.Web.HttpContext.Current.Request.UserHostAddress));
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
                CacheExtension.Set(string.Format(CommonConstants.LOGINFAILEDTIME, System.Web.HttpContext.Current.Request.UserHostAddress), DateTime.Now);
            }
            catch (Exception e)
            {
                Logs.Write(e);
            }
        }

        protected DateTime GetLoginFailedTime()
        {
            DateTime date = DateTime.MinValue;
            try
            {
                if (CacheExtension.Get(string.Format(CommonConstants.LOGINFAILEDTIME, System.Web.HttpContext.Current.Request.UserHostAddress)) is DateTime)
                {
                    date = (DateTime)CacheExtension.Get(string.Format(CommonConstants.LOGINFAILEDTIME, System.Web.HttpContext.Current.Request.UserHostAddress));
                }
            }
            catch (Exception e)
            {
                Logs.Write(e);
            }

            return date;
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

                var getDomainGroupActive = (from m in
                                                domainGroupSection.Cast<DomainGroupElement>().Where(
                                                m => m.Active)
                                        select m);

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
        #endregion
    }
}
