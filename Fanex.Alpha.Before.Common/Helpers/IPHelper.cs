// --------------------------------------------------------------------------
// <copyright file="IPHelper.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.Alpha.Before.Common.Helpers
{
    using Fanex.Logging.Extensions.Web;
    using System.Web;

    /// <summary>
    /// IP Helper.
    /// </summary>
    public static class IPHelper
    {
        public static string ClientIP()
        {
            var ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ??
                System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            if (string.IsNullOrWhiteSpace(ip))
            {
                ExceptionHandler.Handle(new System.Exception("Before login: ip is null or empty"), System.Diagnostics.TraceEventType.Error);
            }

            if (!string.IsNullOrWhiteSpace(ip) && !ip.Equals("::1") && string.IsNullOrWhiteSpace(ip.Split(',')[0]))
            {
                ExceptionHandler.Handle(new System.Exception("Before login: HTTP_X_FORWARDED_FOR ip is null or empty"), System.Diagnostics.TraceEventType.Error);
            }

            ip = string.IsNullOrWhiteSpace(ip) || ip.Equals("::1") || string.IsNullOrWhiteSpace(ip.Split(',')[0]) ? "127.0.0.1" : ip.Split(',')[0];

            return ip;
        }
    }
}
