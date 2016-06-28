// -----------------------------------------------------------------------
// <copyright file="DeviceHelper.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Alpha.Before.Common.Helpers
{
    using System;
    using System.Collections;
    using System.Web;
    using Fanex.Alpha.Before.Infrastructure.Extensions;
    using Fanex.Common;
    using Sunplus.Core.Common.IPs;
    using Sunplus.Core.Common.IPs.DAL;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DeviceHelper
    {
        /// <summary>
        /// Get Request Info.
        /// </summary>
        /// <param name="request">HTTP Request.</param>
        /// <returns>Return result.</returns>
        public static Hashtable GetRequestInfo(HttpRequest request)
        {
            Hashtable requestInfo = ClientRequestInfo(request);

            try
            {
                IPTable infoIp = IpUtils.LocationFromIP(Converter.ToString2(requestInfo["loginip"]));
                requestInfo["countrycode"] = infoIp.CountryLong.Equals("reserved", StringComparison.OrdinalIgnoreCase) ? null : infoIp.CountryLong;
            }
            catch (Exception ex)
            {
                LogExtension.WriteInfo(ex);
            }

            try
            {
                if (request.Browser != null)
                {
                    // Mobile Device Browser
                    if (request.Browser.IsMobileDevice)
                    {
                        requestInfo["browsername"] = string.Format("{0} {1}", request.Browser.Browser, request.Browser.MajorVersion);
                        requestInfo["platform"] = request.Browser.MobileDeviceModel;
                    }
                    else
                    {
                        requestInfo["browsername"] = string.Format(
                            "{0} {1}",
                            request.UserAgent != null && request.UserAgent.ToUpper().Contains("CHROME") ? "Chrome" : request.Browser.Browser,
                            request.Browser.Version);
                        requestInfo["platform"] = GetPlatform();
                    }
                }
            }
            catch (Exception ex)
            {
                LogExtension.WriteInfo(ex);
            }

            return requestInfo;
        }

        /// <summary>
        /// Get Plat form.
        /// </summary>
        /// <returns>Return result.</returns>
        private static string GetPlatform()
        {
            // Create a list of all platform
            string[] platformArrays = new[]
            { 
                "Windows 3", 
                "Win16", 
                "Windows 95", 
                "Win95", 
                "Windows 98", 
                "Win98", 
                "Windows 2000", 
                "Windows XP", 
                "Windows Server 2003", 
                "Windows Vista", 
                "Windows NT", 
                "WinNT", 
                "Windows ME", 
                "Open BSD", 
                "OpenBSD", 
                "Sun OS", 
                "SunOS", 
                "Linux", 
                "Mac OS", 
                "Mac_PowerPC", 
                "Macintosh", 
                "QNX", 
                "QNX", 
                "BeOS", 
                "BeOS", 
                "OS/2", 
                "OS/2" 
            };
            HttpContext context = HttpContext.Current;

            if (context != null && context.Request != null &&
                context.Request.ServerVariables != null &&
                !string.IsNullOrWhiteSpace(context.Request.ServerVariables["HTTP_USER_AGENT"]))
            {
                // Loop through each item in the list created above and check if the header contains that text
                foreach (string flatform in platformArrays)
                {
                    if (context.Request.ServerVariables["HTTP_USER_AGENT"].ToLower().Contains(flatform.ToLower()))
                    {
                        return flatform;
                    }
                }
            }

            return "Unknown";
        }

        /// <summary>
        /// Client Request Info.
        /// </summary>
        /// <param name="request">HTTP Request.</param>
        /// <returns>Return result.</returns>
        private static Hashtable ClientRequestInfo(HttpRequest request)
        {
            var clientIp = IPHelper.ClientIP();
            var countryCode = "Unknown";
            return new Hashtable()
            {
                {
                    "loginip", clientIp
                },
                {
                    "countrycode", countryCode
                },
                {
                    "logindomain", request.ServerVariables["SERVER_NAME"]
                },
                {
                    "browsername", string.Empty
                },
                {
                    "platform", string.Empty
                }
            };
        }
    }
}