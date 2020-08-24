// -----------------------------------------------------------------------
// <copyright file="AuthController.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Alpha.Before.Customer.Controllers
{
    using CaptchaMvc.HtmlHelpers;
    using Fanex.Alpha.Before.Common.Configurations;
    using Fanex.Alpha.Before.Common.Controllers;
    using Fanex.Alpha.Before.Common.Helpers;
    using Fanex.Alpha.Before.Customer.Constants;
    using Fanex.Alpha.Before.Customer.Models;
    using Fanex.Alpha.Before.Customer.Services.SignIn;
    using Logging.Extensions.Web;
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class AuthController : BaseController
    {
        private readonly ISignInService _signInService;
        private readonly string viewPath = "~/Areas/Customer/Views/SignIn/Index.cshtml";

        public AuthController() :
            this(null)
        {
        }

        public AuthController(SignInService signInService)
        {
            _signInService = signInService ?? new SignInService();
        }

        /// <summary>
        /// Index page.
        /// </summary>
        /// <returns>Return view.</returns>
        [HttpGet]
        public ActionResult Index()
        {
            if (IsNeedMappingSslDomain())
            {
                return Redirect("https://" + Request.Headers["Host"]);
            }

            if (Request.QueryString["UMToken"] != null)
            {
                Session["UMToken"] = Request.QueryString["UMToken"];
            }

            var loginResult = _signInService.SignIn();
            ViewBag.FPSToken = FPS.Client.Token.Generate();
            AppSession.ResetRobotCaptchaSession();

            return View(viewPath, loginResult);
        }

        /// <summary>
        /// Index page.
        /// </summary>
        /// <param name="txtUserName">User name.</param>
        /// <param name="txtPassWord">User password.</param>
        /// <param name="hidLanguage">Hidden language.</param>
        /// <param name="code">Encrypt code.</param>
        /// <returns>
        /// Return result.
        /// </returns>
        [HttpPost, ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Index(
            string txtUserName,
            string txtPassWord,
            string hidLanguage,
            string code,
            string browserSize,
            string deviceId = "N/A")
        {
            AssignSettingLanguage(hidLanguage);
            ViewBag.FPSToken = FPS.Client.Token.Generate();
            AppSession.ResetRobotCaptchaSession();
            bool validCaptcha = false;
            if (_signInService.IsShowReCaptcha())
            {
                try
                {
                    validCaptcha = this.IsCaptchaValid(null);
                }
                catch
                {
                    validCaptcha = false;
                }
            }

            var loginResult = _signInService.SignInWithRecaptcha(txtUserName, txtPassWord, hidLanguage, validCaptcha, code, deviceId, browserSize);

            if (loginResult != null && loginResult.ErrCode != SignInErrorContants.Error2)
            {
                if (loginResult.ErrCode == -1)
                {
                    FPSTracking(loginResult);

                    return string.IsNullOrEmpty(loginResult.AfterLoginUrl) ?
                        RedirectToAction("Index") :
                        (ActionResult)Redirect(loginResult.AfterLoginUrl);
                }

                ViewBag.ShowReCaptcha = _signInService.IsShowReCaptcha();
            }

            return View(viewPath, loginResult);
        }

        [HttpPost]
        public async Task<ActionResult> ValidateGoogleCaptcha(string token)
        {
            var googleReCaptcha = Common.Helpers.UrlHelper.SiteGroupSetting.GoogleReCaptcha;

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

        public RedirectResult AutomationTest(string username, string password, string captcha, string deviceId, string browserSize)
        {
            var loginResult = _signInService.AutomationTest(username, password, captcha, deviceId, browserSize);

            if (loginResult.ErrCode == -1)
            {
                return Redirect(loginResult.AfterLoginUrl);
            }

            return Redirect("~/Auth/Index");
        }

        /// <summary>
        /// Function to send login status to FPS.
        /// </summary>
        /// <param name="loginResult">Login result.</param>
        private static void FPSTracking(SignInResult loginResult)
        {
            try
            {
                if (!AppSetting.IsEnableFPS)
                {
                    return;
                }

                const string loginAction = "login";
                var loginMessage = string.Empty;
                var username = loginResult?.Username;
                var fpsSubscirber = ConfigurationManager.AppSettings["FPS_Subscriber"];
                var fpsErrorMessage = string.Empty;
                var fpsErrorCode = string.Empty;

                if (loginResult == null)
                {
                    loginMessage = "failed";
                }
                else if (loginResult.ErrCode != -1)
                {
                    loginMessage = string.Format("{0}", loginResult.ErrMsg);
                }
                else
                {
                    loginMessage = "successfully";
                }

                new FPS.Client.Service().CheckTransaction(
                    username?.Trim(),
                    loginAction,
                    loginMessage,
                    fpsSubscirber,
                    out fpsErrorMessage,
                    out fpsErrorCode);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }
        }

        private bool IsNeedMappingSslDomain()
        {
            var sslPorts = ConfigurationManager.AppSettings["SSLPorts"];
            var sslDomains = ConfigurationManager.AppSettings["SSLDomains"];
            if (sslPorts != null)
            {
                var isPortNeedForSsl = sslPorts.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Any(port => port.Equals(Request.ServerVariables["SERVER_PORT"], StringComparison.OrdinalIgnoreCase));

                if (!isPortNeedForSsl && sslDomains != null)
                {
                    var domains = sslDomains.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (domains.Any(domain => domain.Equals(Request.Headers["Host"], StringComparison.OrdinalIgnoreCase)))
                    {
                        return true;
                    }
                }
            }

            return false;
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
    }
}