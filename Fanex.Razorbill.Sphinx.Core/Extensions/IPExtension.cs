// -----------------------------------------------------------------------
// <copyright file="IPExtension.cs" company="Nexcel Solution">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Razorbill.Sphinx.Core.Extensions
{
    using Sunplus.Agent.Model.Common;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class IPExtension
    {
        public static string CustomerIp()
        {
            return AgentIPFormat.GetClientIP().Equals("::1") ? "127.0.0.1" : AgentIPFormat.GetClientIP();
        }
    }
}
