

using System.Configuration;
using DNAide.Utils;
using Fanex.Razorbill.Sphinx.Core.Configurations;

namespace Fanex.Razorbill.Sphinx.Web.Controllers.Login
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Web.Mvc;
    using Fanex.Razorbill.Sphinx.Core.Extensions;
    using Fanex.Razorbill.Sphinx.Service.Common.UnderMaintenance;
    using Fanex.Razorbill.Sphinx.Service.UserLogin;

    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public class LoginController : BaseController
    {   
        public LoginController()
        {
        }
        
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
                return View("~/Views/Login/Template/Default/Default.cshtml");
            
            return View(string.Format("~/Views/Login/Template/{0}/Index.cshtml", loginResult.LoginTemplate), loginResult);
        }

        public ActionResult GetNewCaptcha()
        {

            var loginService = new UserLoginService();
            var graphic = loginService.GetNewCaptcha();

            MemoryStream ms = new MemoryStream();
            return new FileStreamResult(ms, "image/Jpeg");

        }

        [HttpPost]
        public ActionResult Index(string txtUserName, string txtPassWord, string hidLanguage, string txtCaptcha, string code, string __di)
        {
            var loginService = new UserLoginService();
            var loginResult = loginService.SignIn(txtUserName, txtPassWord, hidLanguage, txtCaptcha, code);

            try
            {
                bool loginFailed = loginResult == null || loginResult.ErrCode != -1;
                FPS.Client.Service fps = new FPS.Client.Service();
                fps.CheckTransaction(txtUserName, loginFailed ? "login-failed" : "login", "Agency", null);
            }
            catch (Exception ex)
            {
            }

            //return Json(loginResult);

            //return PartialView("~/Views/Login/Transfer.cshtml", loginResult);
            return PartialView(string.Format("~/Views/Login/Template/{0}/Index.cshtml", loginResult.LoginTemplate), loginResult);
        }
    }
}