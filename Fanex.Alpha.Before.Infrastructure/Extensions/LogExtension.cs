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
    using System.Collections;
    using System.Data.SqlClient;
    using System.Threading;

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
            var fullError = FullExceptionToString(ex);
            Logger.Log.Error(fullError);
        }

        public static void WriteInfo(Exception ex)
        {
            var fullError = FullExceptionToString(ex);
            Logger.Log.Info(fullError);
        }

        private static string FullExceptionToString(Exception ex, int customerId = 0)
        {
            if (ex == null || HttpContext.Current == null)
            {
                return string.Empty;
            }

            HttpRequest httpRequest = HttpContext.Current.Request;
            return string.Join("\r\n",
                ExceptionToString(ex, httpRequest, customerId),
                "EXCEPTION DATA:" + ExceptionDataToString(ex.Data),
                "BROWSER CAPABILITIES:" + BrowserInformation(httpRequest.Browser),
                SqlExceptionToString(ex));
        }

        private static string ExceptionDataToString(IDictionary dictionary)
        {
            string message = string.Empty;
            if (dictionary != null)
            {
                foreach (var key in dictionary.Keys)
                {
                    message += string.Format("\r\n{0}: {1}", key, dictionary[key]);
                }
            }

            return message;
        }

        private static string SqlExceptionToString(Exception ex)
        {
            if (ex is SqlException)
            {
                return GetSqlExceptionInfo((SqlException)ex);
            }
            else
            {
                if (ex.InnerException is SqlException)
                {
                    return GetSqlExceptionInfo((SqlException)ex.InnerException);
                }
            }

            return string.Empty;
        }

        private static string GetSqlExceptionInfo(SqlException ex)
        {
            return string.Join("\r\n",
                    "PROCEDURE: " + ex.Procedure,
                    "LINE NUMBER: " + ex.LineNumber,
                    "SERVER: " + ex.Server);
        }

        private static string ExceptionToString(Exception ex, HttpRequest httpRequest, int customerId)
        {
            string url = httpRequest.Url.OriginalString;

            string formString = string.Empty;
            if (httpRequest.Form.Count > 0)
            {
                formString = httpRequest.Form.ToString();
            }

            string message = string.Join("\r\n",
                ex.Message,
                "URL: " + url,
                "REFERRER: " + httpRequest.UrlReferrer,
                "FORM: " + formString,
                "SOURCE: " + ex.Source,
                "TARGETSITE: " + ex.TargetSite,
                "DETAIL: " + ex,
                "CULTURE: " + Thread.CurrentThread.CurrentUICulture,
                "CLIENT IP: " + CustomerIp());

            if (customerId != 0)
            {
                message += "\r\nCUSTID: " + customerId;
            }

            return message;
        }

        private static string BrowserInformation(HttpBrowserCapabilities browser)
        {
            if (browser == null)
            {
                return string.Empty;
            }

            return string.Join("\r\n",
                "Type: " + browser.Type,
                "Name: " + browser.Browser,
                "Version: " + browser.Version,
                "Major Version: " + browser.MajorVersion,
                "Minor Version: " + browser.MinorVersion,
                "Platform: " + browser.Platform,
                "Is Beta: " + browser.Beta,
                "Is Crawler: " + browser.Crawler,
                "Is AOL: " + browser.AOL,
                "Is Win16: " + browser.Win16,
                "Is Win32: " + browser.Win32,
                "Supports Frames: " + browser.Frames,
                "Supports Tables: " + browser.Tables,
                "Supports Cookies: " + browser.Cookies);
        }

        private static string CustomerIp()
        {
            var ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            return ip.Equals("::1") ? "127.0.0.1" : (string.IsNullOrEmpty(ip) ? "127.0.0.1" : ip.Split(',')[0]);
        }
    }
}