// -----------------------------------------------------------------------
// <copyright file="SignInController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Alpha.Before.Customer.Controllers
{
    using System;
    using System.Configuration;
    using System.Web.Mvc;
    using CaptchaMvc.HtmlHelpers;
    using Fanex.Alpha.Before.Common.Controllers;
    using Fanex.Alpha.Before.Customer.Models;
    using Fanex.Alpha.Before.Customer.Services.SignIn;
    using Fanex.Alpha.Before.Infrastructure.Extensions;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SignInController : BaseController
    {
        private ISignInService _signInService;
        private string _templatePath = "~/Areas/Customer/Views/SignIn/Template/{0}/Index.cshtml";

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
            var loginResult = _signInService.SignIn();

            return View(string.Format(_templatePath, loginResult.LoginTemplate), loginResult);
        }

        /// <summary>
        /// Index page.
        /// </summary>
        /// <param name="txtUserName">User name.</param>
        /// <param name="txtPassWord">User password.</param>
        /// <param name="hidLanguage">Hidden language.</param>
        /// <param name="code">Encrypt code.</param>
        /// <param name="__di">FPS validate code.</param>
        /// <returns>
        /// Return result.
        /// </returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Index(string txtUserName, string txtPassWord, string hidLanguage, string code, string __di)
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

            var loginResult = _signInService.SignInWithRecaptcha(txtUserName, txtPassWord, hidLanguage, validCaptcha, code);

            if (loginResult != null && loginResult.ErrCode != -2)
            {
                FPSTracking(txtUserName, loginResult);
                ViewBag.ShowReCaptcha = _signInService.IsShowReCaptcha();
            }
            
            return PartialView(string.Format(_templatePath, loginResult.LoginTemplate), loginResult);
        }

        public RedirectResult AutomationTest(string username, string password, string captcha)
        {
            var loginResult = _signInService.AutomationTest(username, password, captcha);

            if (loginResult.ErrCode == -1)
            {
                return Redirect(loginResult.AfterLoginUrl);
            }

            return Redirect("~/Customer/SignIn");
        }

        /// <summary>
        /// Function to send login status to FPS.
        /// </summary>
        /// <param name="username">The Username.</param>
        /// <param name="loginResult">Login result.</param>
        private static void FPSTracking(string username, SignInResult loginResult)
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
                    username,
                    loginAction,
                    loginMessage,
                    fpsSubscirber,
                    out fpsErrorMessage,
                    out fpsErrorCode);
            }
            catch (Exception ex)
            {
                LogExtension.ExceptionHandler(ex);
            }
        }
    }
}
