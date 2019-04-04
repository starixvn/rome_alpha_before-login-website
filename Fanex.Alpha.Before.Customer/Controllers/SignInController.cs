// -----------------------------------------------------------------------
// <copyright file="SignInController.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Alpha.Before.Customer.Controllers
{
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Web.Mvc;
    using CaptchaMvc.HtmlHelpers;
    using Fanex.Alpha.Before.Common.Controllers;
    using Fanex.Alpha.Before.Customer.Models;
    using Fanex.Alpha.Before.Customer.Services.SignIn;
    using Logging.Extensions.Web;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SignInController : BaseController
    {
        private readonly ISignInService _signInService;
        private readonly string viewPath = "~/Areas/Customer/Views/SignIn/Index.cshtml";

        public SignInController() :
            this(null)
        {
        }

        public SignInController(SignInService signInService)
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
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Index(
            string txtUserName,
            string txtPassWord,
            string hidLanguage,
            string code,
            string browserSize,
            string deviceId = "N/A")
        {
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

            if (loginResult != null && loginResult.ErrCode != -2)
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

        public RedirectResult AutomationTest(string username, string password, string captcha, string deviceId, string browserSize)
        {
            var loginResult = _signInService.AutomationTest(username, password, captcha, deviceId, browserSize);

            if (loginResult.ErrCode == -1)
            {
                return Redirect(loginResult.AfterLoginUrl);
            }

            return Redirect("~/Customer/SignIn");
        }

        /// <summary>
        /// Function to send login status to FPS.
        /// </summary>
        /// <param name="loginResult">Login result.</param>
        private static void FPSTracking(SignInResult loginResult)
        {
            try
            {
                var loginAction = "login";
                var loginMessage = "successfully";
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

                new FPS.Client.Service().CheckTransaction(
                    loginResult.Username,
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
    }
}