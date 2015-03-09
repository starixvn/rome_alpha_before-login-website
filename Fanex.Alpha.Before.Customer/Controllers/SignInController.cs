// -----------------------------------------------------------------------
// <copyright file="SignInController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Alpha.Before.Customer.Controllers
{
    using System;
    using System.IO;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using CaptchaMvc.HtmlHelpers;
    using Fanex.Alpha.Before.Common.Controllers;
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

        ///// <summary>
        ///// Index page.
        ///// </summary>
        ///// <param name="txtUserName">User name.</param>
        ///// <param name="txtPassWord">User password.</param>
        ///// <param name="hidLanguage">Hidden language.</param>
        ///// <param name="txtCaptcha">Captcha value.</param>
        ///// <param name="code">Encrypt code.</param>
        ///// <param name="__di">FPS validate code.</param>
        ///// <returns>Return result.</returns>
        //[AcceptVerbs(HttpVerbs.Post)]
        //[ValidateInput(false)]
        //public ActionResult Index(string txtUserName, string txtPassWord, string hidLanguage, string txtCaptcha, string code, string __di)
        //{
        //    var loginResult = _signInService.SignIn(txtUserName, txtPassWord, hidLanguage, txtCaptcha, code);
        //    try
        //    {
        //        new FPS.Client.Service().CheckTransaction(
        //            txtUserName, 
        //            (loginResult == null || loginResult.ErrCode != -1) ? "login-failed" : "login", 
        //            "Agency");                
        //    }
        //    catch (Exception ex)
        //    {
        //        LogExtension.ExceptionHandler(ex);
        //    }

        //    return PartialView(string.Format(_templatePath, loginResult.LoginTemplate), loginResult);
        //}

        /// <summary>
        /// Index page.
        /// </summary>
        /// <param name="txtUserName">User name.</param>
        /// <param name="txtPassWord">User password.</param>
        /// <param name="hidLanguage">Hidden language.</param>
        /// <param name="txtCaptcha">Captcha value.</param>
        /// <param name="code">Encrypt code.</param>
        /// <param name="__di">FPS validate code.</param>
        /// <returns>Return result.</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateInput(false)]
        public ActionResult Index(string txtUserName, string txtPassWord, string hidLanguage)
        {
            bool validCaptcha = false;
            try
            {
                validCaptcha = this.IsCaptchaValid(null);
            }
            catch
            {
                validCaptcha = false;
            }

            var loginResult = _signInService.SignInWithRecaptcha(txtUserName, txtPassWord, hidLanguage, validCaptcha);
            
            try
            {
                new FPS.Client.Service().CheckTransaction(
                    txtUserName,
                    (loginResult == null || loginResult.ErrCode != -1) ? "login-failed" : "login",
                    "Agency");
            }
            catch (Exception ex)
            {
                LogExtension.ExceptionHandler(ex);
            }

            ViewBag.ShowReCaptcha = _signInService.IsShowReCaptcha();
            return PartialView(string.Format(_templatePath, loginResult.LoginTemplate), loginResult);
        }

        public ActionResult NewCaptcha()
        {
            var graphic = _signInService.NewCaptcha();

            MemoryStream ms = new MemoryStream();
            return new FileStreamResult(ms, "image/Jpeg");
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
    }
}
