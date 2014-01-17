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
    using Fanex.Razorbill.Sphinx.Core.Configurations;

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

            HttpRequest request = HttpContext.Current.Request;
            // hide sensitive information in POST or GET parameters
            string formString = string.Empty, queryString = string.Empty;
            if (request.Form.Count > 0)
            {
                formString = request.Form.ToString();
                formString = ReplaceSensitiveInformation(formString, WebSettings.SensitiveParameters);
            }

            if (request.QueryString.Count > 0)
            {
                queryString = request.QueryString.ToString();
                queryString = ReplaceSensitiveInformation(queryString, WebSettings.SensitiveParameters);
            }
            
            string message = ex.Message +
                              "<br>\r\nSOURCE: " + ex.Source +
                              "<br>\r\nDOMAIN: " + request.Headers["Host"] +
                              "<br>\r\nREFERRER: " + request.UrlReferrer +
                              "<br>\r\nFORM: " + formString +
                              "<br>\r\nQUERYSTRING: " + queryString +
                              "<br>\r\nTARGETSITE: " + ex.TargetSite +
                              "<br>\r\nSTACKTRACE: " + ex.StackTrace;
            Fanex.Logging.Logger.Log.Error(message);
        }

        public static string ReplaceSensitiveInformation(string parameterString, params string[] sensitiveString)
        {
            if (string.IsNullOrEmpty(parameterString) || sensitiveString == null || sensitiveString.Length == 0)
            {
                return parameterString;
            }

            string splitChar = "&";
            string[] paramsArray = parameterString.Split(new string[] { splitChar }, StringSplitOptions.RemoveEmptyEntries);

            int length;
            if (paramsArray == null || (length = paramsArray.Length) == 0)
            {
                return parameterString;
            }

            string parameter;
            for (int i = 0; i < length; i++)
            {
                parameter = paramsArray[i].ToLower();
                foreach (string sensitive in sensitiveString)
                {
                    if (parameter.Contains(sensitive))
                    {
                        int indexOf = parameter.IndexOf('=');
                        if (indexOf >= 0)
                        {
                            paramsArray[i] = paramsArray[i].Substring(0, indexOf + 1) + "******";
                        }

                        break;
                    }
                }
            }

            return string.Join(splitChar, paramsArray);
        }
    }
}
