// --------------------------------------------------------------------------
// <copyright file="IPHelper.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.Alpha.Before.Common.Helpers
{
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

            return (string.IsNullOrEmpty(ip) || ip.Equals("::1")) ? "127.0.0.1" : ip.Split(',')[0];
        }
    }
}
