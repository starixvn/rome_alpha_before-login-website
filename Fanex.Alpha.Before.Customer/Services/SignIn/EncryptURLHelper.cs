namespace Fanex.Alpha.Before.Customer.Services.SignIn
{
    using System;
    using System.Web;
    using System.Web.Security;
    using Fanex.Alpha.Before.Common.Configurations;
    using Fanex.Alpha.Before.Common.Helpers;
    using Fanex.Alpha.Before.Customer.Configurations;
    using Fanex.Alpha.Before.Customer.Models;
    using RSACryptoPad;

    public static class EncryptURLHelper
    {
        public const string UTMS = "__utms";
        private const string IbcpKeyPath = "~/IbcKeys/ibcpkey.snk";
        private const int DWKeySize = 1024;

        public static string BuildAfterURL(HttpContext httpContext)
            => AppSession.CustId == 0
            ? FormsAuthentication.LoginUrl
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

        public static string BuildAfterURL(
            HttpContext httpContext,
            string encryptPassword,
            VerifySignInResult verifySignInResult,
            string deviceId,
            string browserSize)
        {
            var parameter = string.Format(
                "{0}&{1}&{2}&{3}&{4}&{5}&__utms&{6}&{7}&{8}&{9}&{10}",
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
                AppSession.EncryptCode);

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
    }
}
