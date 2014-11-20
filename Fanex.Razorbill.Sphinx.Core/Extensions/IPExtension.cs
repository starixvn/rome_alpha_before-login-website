// -----------------------------------------------------------------------
// <copyright file="IPExtension.cs" company="Nexcel Solution">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Razorbill.Sphinx.Core.Extensions
{
    using System.Web;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class IPExtension
    {
        public static string CustomerIp()
        {
            var ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            return ip.Equals("::1") ? "127.0.0.1" : (string.IsNullOrEmpty(ip) ? "127.0.0.1" : ip.Split(',')[0]);
        }
    }
}
