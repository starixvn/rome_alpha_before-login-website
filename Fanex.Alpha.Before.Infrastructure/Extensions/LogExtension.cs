// --------------------------------------------------------------------------
// <copyright file="LogExtension.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.Alpha.Before.Infrastructure.Extensions
{
    using System;
    using System.Web;
    using Fanex.Logging;

    /// <summary>
    /// Log Extensions.
    /// </summary>
    public static class LogExtension
    {
        /// <summary>
        /// Exceptions the handler.
        /// </summary>
        /// <param name="ex">The exception.</param>
        public static void ExceptionHandler(Exception ex)
        {
            var request = HttpContext.Current.Request;
            string messageTemplate = "{0}<br>\r\nSOURCE: {1}<br>\r\nDOMAIN: {2}<br>\r\nREFERRER: {3}<br>\r\nQUERYSTRING: {4}<br>\r\nTARGETSITE: {5}<br>\r\nSTACKTRACE: {6}";
            Logger.Log.Error(string.Format(
                messageTemplate,
                ex.Message,
                ex.Source,
                request.Headers["Host"],
                request.UrlReferrer,
                request.QueryString.ToString(),
                ex.TargetSite,
                ex.StackTrace));
        }
    }
}
