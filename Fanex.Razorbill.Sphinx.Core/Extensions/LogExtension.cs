// -----------------------------------------------------------------------
// <copyright file="LogExtension.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Razorbill.Sphinx.Core.Extensions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Web;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public class LogExtension
    {
        public static void ExceptionHandler(Exception ex)
        {
            if (ex is HttpException)
            {
                return;
            }
            
            string message = ex.Message +
                              "<br>\r\nSOURCE: " + ex.Source +
                              "<br>\r\nDOMAIN: " + HttpContext.Current.Request.Headers["Host"] +
                              "<br>\r\nREFERRER: " + HttpContext.Current.Request.UrlReferrer +
                              "<br>\r\nFORM: " + HttpContext.Current.Request.Form.ToString() +
                              "<br>\r\nQUERYSTRING: " + HttpContext.Current.Request.QueryString.ToString() +
                              "<br>\r\nTARGETSITE: " + ex.TargetSite +
                              "<br>\r\nSTACKTRACE: " + ex.StackTrace;
            Fanex.Logging.Logger.Log.Error(message);
        }
    }
}
