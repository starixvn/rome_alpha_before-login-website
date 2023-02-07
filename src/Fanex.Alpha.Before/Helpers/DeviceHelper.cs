using System.Collections;
using Fanex.Logging;
using UAParser;

namespace Fanex.Alpha.Before.Helpers;

public static class DeviceHelper
{
    public static Hashtable GetRequestInfo(HttpRequest request, string browserSize, string clientIP)
    {
        Hashtable requestInfo = InitClientRequestInfo(request, clientIP);

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

    private static Hashtable InitClientRequestInfo(HttpRequest request, string clientIP)
    {
        return new Hashtable
        {
            ["loginip"] = clientIP,
            ["countrycode"] = "Unknown",
            // ["logindomain"] = request.ServerVariables["SERVER_NAME"],
            ["logindomain"] = request.Host.Value,
            ["browsername"] = string.Empty,
            ["platform"] = string.Empty
        };
    }

    private static void GetRequestInfoFromContext(HttpRequest request, string browserSize, Hashtable requestInfo)
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

        requestInfo["browsername"] = browser;
        requestInfo["platform"] = $"{operateSystem}, Device: {device}";
    }
}