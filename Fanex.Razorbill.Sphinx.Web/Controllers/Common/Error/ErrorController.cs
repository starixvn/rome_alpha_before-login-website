// -----------------------------------------------------------------------
// <copyright file="ErrorController.cs" company="Nexcel Solution">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Razorbill.Sphinx.Web.Controllers.Common
{
    using System.Web.Mvc;
    using Fanex.Razorbill.Sphinx.Core.Configurations;
    using Fanex.Razorbill.Sphinx.Core.Constants;
    using Fanex.Razorbill.Sphinx.Core.Extensions;
    using Fanex.Razorbill.Sphinx.Core.Models;

    /// <summary>
    /// Error Controller.
    /// </summary>
    public class ErrorController : Controller
    {        
        /// <summary>
        /// Index page.
        /// </summary>
        /// <param name="errCode">Error code.</param>
        /// <returns>Return view.</returns>
        public ActionResult Index(int errCode = 500)
        {
            var result = new Result()
            {
                ErrorCode = ErrorContants.ErrorDefault,
                ErrorMessage = ResourceExtension.GetResource(WebSettings.ResourceName, "HttpError_Default")
            };

            switch (errCode)
            {
                case ErrorContants.Error403:
                    result.ErrorCode = ErrorContants.Error403;
                    result.ErrorMessage = ResourceExtension.GetResource(WebSettings.ResourceName, "HttpError_403");
                    break;
                case ErrorContants.Error404:
                    result.ErrorCode = ErrorContants.Error404;
                    result.ErrorMessage = ResourceExtension.GetResource(WebSettings.ResourceName, "HttpError_404");
                    break;
                default:
                    break;
            }

            return View("~/Views/Common/Error/Index.cshtml", result);
        }
    }
}