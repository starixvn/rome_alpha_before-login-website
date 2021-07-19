namespace Fanex.Alpha.Before.Customer.Services.SignIn
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Web;
    using System.Web.Security;
    using Fanex.Alpha.Before.Common.Configurations;
    using Fanex.Alpha.Before.Common.Helpers;
    using Fanex.Alpha.Before.Customer.Configurations;
    using Fanex.Alpha.Before.Customer.Models;
    using Fanex.Alpha.Before.Customer.Resources;
    using RSACryptoPad;

    public static class EncryptURLHelper
    {
        public const string UTMS = "__utms";
        private const string IbcpKeyPath = "~/IbcKeys/ibcpkey.snk";
        private const int DWKeySize = 1024;

        public static void WriteLoginLogs(int superId, string message)
        {
            if (SuperIds.Any(id => id == superId))
            {
                Logging.Logger.Log.Warn(message);
            }
        }

        public static string BuildAfterURL(HttpContext httpContext)
        {
            WriteLoginLogs(AppSession.SuperId, $"After input captcha, sessionId: {httpContext.Session.SessionID}, custId:{AppSession.CustId}");
            var url = AppSession.CustId == 0
            ? $"{FormsAuthentication.LoginUrl}?r={Labels.inputCaptchaTooLong}"
            : BuildAfterURL(
                httpContext,
                AppSession.EncryptPassword,
                new VerifySignInResult
                {
                    CustId = AppSession.CustId,
                    Username = AppSession.Username,
                    SubAccId = AppSession.SubAccId,
                    IsNickname = AppSession.IsNickname,
                    Nickname = AppSession.Nickname,
                    LoginTime = DateTime.Now
                },
                AppSession.DeviceId,
                AppSession.BrowserSize);

            AppSession.ResetRobotCaptchaSession();

            return url;
        }

        public static string BuildAfterURL(
            HttpContext httpContext,
            string encryptPassword,
            VerifySignInResult verifySignInResult,
            string deviceId,
            string browserSize)
        {
            WriteLoginLogs(verifySignInResult.SuperId, $"Build after url, sessionId: {httpContext.Session.SessionID}, custId:{verifySignInResult.CustId}");

            var parameter = string.Format(
                "{0}&{1}&{2}&{3}&{4}&{5}&__utms&{6}&{7}&{8}&{9}&{10}&{11}",
                verifySignInResult.CustId,
                verifySignInResult.SubAccId,
                verifySignInResult.LoginTime.Ticks,
                encryptPassword,
                verifySignInResult.Nickname,
                verifySignInResult.IsNickname,
                AppSession.Language,
                NumberCrossCookie(httpContext),
                deviceId,
                browserSize,
                AppSession.EncryptCode,
                AppSession.DarkModeName);

            parameter = EncryptParameter(httpContext, parameter);
            var url = $"{UrlHelper.SiteGroupSetting.AfterUrl}{CustomerAppSetting.AfterLoginAuthorizationPage}?params={parameter}";

            return AddUMToken(httpContext, url);
        }

        private static bool NumberCrossCookie(HttpContext httpContext)
        {
            var utmsCookie = httpContext.Request.Cookies.Get(UTMS);

            return utmsCookie == null || utmsCookie.Value == AppSession.CookieCode;
        }

        private static string EncryptParameter(HttpContext httpContext, string parameter)
            => new EncryptionThread().EncryptString(parameter, DWKeySize, GetPhysicalIbcpKeyPath(httpContext));

        private static string GetPhysicalIbcpKeyPath(HttpContext httpContext)
            => httpContext.Server.MapPath(IbcpKeyPath);

        private static string AddUMToken(HttpContext httpContext, string url)
        {
            var umToken = httpContext.Session["UMToken"];

            return umToken == null
                ? url
                : $"{url}&UMToken={umToken}";
        }

        private static IEnumerable<int> SuperIds =
            string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["Robot:TrackLogSuperIds"]) ? Enumerable.Empty<int>() :
            ConfigurationManager.AppSettings["Robot:TrackLogSuperIds"]
            .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse);
    }
}
