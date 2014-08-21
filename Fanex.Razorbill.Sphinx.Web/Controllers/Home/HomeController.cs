// -----------------------------------------------------------------------
// <copyright file="HomeController.cs" company="Nexcel Solution">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Razorbill.Sphinx.Web.Controllers.Home
{
    using System.Web.Mvc;

    /// <summary>
    /// Home Controller.
    /// </summary>
    public class HomeController : BaseController
    {
        public HomeController()
        {
        }

        public ActionResult Index()
        {
            return RedirectToAction("Index", "Login");
        }
    }
}
