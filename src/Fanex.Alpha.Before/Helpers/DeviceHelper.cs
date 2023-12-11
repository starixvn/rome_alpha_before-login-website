using Fanex.Alpha.Before.Models;
using Fanex.Logging;
using UAParser;

namespace Fanex.Alpha.Before.Helpers;

public static class DeviceHelper
{
    public static RequestInfo GetRequestInfo(HttpRequest request, string browserSize, string clientIP)
    {
        RequestInfo requestInfo = InitClientRequestInfo(request, clientIP);

        try
        {
            GetRequestInfoFromContext(request, browserSize, requestInfo);
        }
        catch (Exception ex)
        {
            Logger.Log.Info(ex.Message);
        }

        return requestInfo;
    }

    private static RequestInfo InitClientRequestInfo(HttpRequest request, string clientIP)
        => new RequestInfo
        {
            LoginIP = clientIP,
            CountryCode = "Unknown",
            // LoginDomain = request.ServerVariables["SERVER_NAME"],
            LoginDomain = request.Host.Value,
            BrowserName = string.Empty,
            Platform = string.Empty
        };

    private static void GetRequestInfoFromContext(HttpRequest request, string browserSize, RequestInfo requestInfo)
    {
        var userAgent = request.Headers["User-Agent"].ToString() ?? string.Empty;
        var userAgentParser = Parser.GetDefault();
        var clientInfo = userAgentParser.Parse(userAgent);
        var clientUserAgent = clientInfo.UA;
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

        requestInfo.BrowserName = browser;
        requestInfo.Platform = $"{operateSystem}, Device: {device}";
    }
}