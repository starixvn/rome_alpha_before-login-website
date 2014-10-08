// -----------------------------------------------------------------------
// <copyright file="LoginController.cs" company="Nexcel Solution">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Razorbill.Sphinx.Web.Controllers.Login
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Web.Mvc;
    using Fanex.Razorbill.Sphinx.Core.Extensions;
    using Fanex.Razorbill.Sphinx.Service.UserLogin;

    /// <summary>
    /// Login Controller.
    /// </summary>
    public class LoginController : BaseController
    {   
        public LoginController()
        {
        }
        
        /// <summary>
        /// Index page.
        /// </summary>
        /// <returns>Return view.</returns>
        [HttpGet]
        public ActionResult Index()
        {
            var startEventDate = Convert.ToDateTime(ConfigurationManager.AppSettings["SpecialEventStart"]);
            var endEventDate = Convert.ToDateTime(ConfigurationManager.AppSettings["SpecialEventEnd"]);
            var currentDate = DateTime.Now;

            var loginService = new UserLoginService();
            var loginResult = loginService.SignIn();

            bool havingFestival = currentDate >= startEventDate && currentDate <= endEventDate;

            if (!havingFestival)
            {
                return View("~/Views/Login/Template/Default/Default.cshtml");
            }

            return View(string.Format("~/Views/Login/Template/{0}/Index.cshtml", loginResult.LoginTemplate), loginResult);
        }

        public ActionResult GetNewCaptcha()
        {
            var loginService = new UserLoginService();
            var graphic = loginService.GetNewCaptcha();

            MemoryStream ms = new MemoryStream();
            return new FileStreamResult(ms, "image/Jpeg");
        }

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
        [HttpPost]
        public ActionResult Index(string txtUserName, string txtPassWord, string hidLanguage, string txtCaptcha, string code, string __di)
        {
            var loginService = new UserLoginService();
            var loginResult = loginService.SignIn(txtUserName, txtPassWord, hidLanguage, txtCaptcha, code);

            try
            {
                bool loginFailed = loginResult == null || loginResult.ErrCode != -1;
                FPS.Client.Service fps = new FPS.Client.Service();
                fps.CheckTransaction(txtUserName, loginFailed ? "login-failed" : "login", "Agency");
            }
            catch (Exception ex)
            {
                LogExtension.ExceptionHandler(ex);
            }

            return PartialView(string.Format("~/Views/Login/Template/{0}/Index.cshtml", loginResult.LoginTemplate), loginResult);
        }
    }
}