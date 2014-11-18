// -----------------------------------------------------------------------
// <copyright file="LimitedCountryAccessController.cs" company="Nexcel Solution">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Razorbill.Sphinx.Web.Controllers.Common
{
    using System.Web.Mvc;

    public class LimitedCountryAccessController : Controller
    {
        /// <summary>
        /// Index page.
        /// </summary>
        /// <returns>Return error page.</returns>
        public ActionResult Index()
        {
            return View("~/Views/Common/LimitedCountryAccess/Index.cshtml");
        }
    }
}