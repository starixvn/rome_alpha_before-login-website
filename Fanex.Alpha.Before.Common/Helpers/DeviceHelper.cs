namespace Fanex.Alpha.Before.Common.Helpers
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Web;
    using Fanex.Common;
    using Logging.Extensions.Web;
    using Sunplus.Core.Common.IPs;
    using Sunplus.Core.Common.IPs.DAL;
    using UAParser;

    public static class DeviceHelper
    {
        public static Hashtable GetRequestInfo(HttpRequest request, string browserSize)
        {
            Hashtable requestInfo = InitClientRequestInfo(request);

            try
            {
                IPTable infoIp = IpUtils.LocationFromIP(Converter.ToString2(requestInfo["loginip"]));
                requestInfo["countrycode"] = infoIp.CountryLong.Equals("reserved", StringComparison.OrdinalIgnoreCase) ? null : infoIp.CountryLong;
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, TraceEventType.Information);
            }

            try
            {
                GetRequestInfoFromContext(request, browserSize, requestInfo);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, TraceEventType.Information);
            }

            return requestInfo;
        }

        private static Hashtable InitClientRequestInfo(HttpRequest request)
        {
            return new Hashtable
            {
                ["loginip"] = IPHelper.ClientIP(),
                ["countrycode"] = "Unknown",
                ["logindomain"] = request.ServerVariables["SERVER_NAME"],
                ["browsername"] = string.Empty,
                ["platform"] = string.Empty
            };
        }

        private static void GetRequestInfoFromContext(HttpRequest request, string browserSize, Hashtable requestInfo)
        {
            var userAgent = request.UserAgent ?? string.Empty;
            var userAgentParser = Parser.GetDefault();
            var clientInfo = userAgentParser.Parse(userAgent);
            var clientUserAgent = clientInfo.UserAgent;
            var clientPatch = clientUserAgent.Patch ?? "0";

            var browser = $"{clientUserAgent.Family} {clientUserAgent.Major}.{clientUserAgent.Minor}.{clientPatch}";
            if (!string.IsNullOrWhiteSpace(browserSize))
            {
                browser += $", {browserSize}";
            }

            var device = $"{clientInfo.Device.Family}, IsCrawler: {clientInfo.Device.IsSpider}";

            var majorVersion = clientInfo.OS.Major ?? "0";
            var minorVersion = clientInfo.OS.Minor ?? "0";
            var osVersion = clientInfo.OS.Patch ?? "0";
            var operateSystem = $"{clientInfo.OS.Family} {majorVersion}.{minorVersion}.{osVersion}";

            requestInfo["browsername"] = browser;
            requestInfo["platform"] = $"{operateSystem}, Device: {device}";
        }
    }
}