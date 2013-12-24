

using Fanex.Razorbill.Sphinx.Core.Configurations;
using Fanex.Razorbill.Sphinx.Core.Extensions;

namespace Fanex.Razorbill.Sphinx.Web.Controllers.Common
{
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Mvc;
    using Fanex.Razorbill.Sphinx.Core.Constants;
    using Fanex.Razorbill.Sphinx.Core.Models;
    

    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public class ErrorController: Controller
    {
        
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