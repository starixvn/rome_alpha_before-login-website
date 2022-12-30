namespace Fanex.Alpha.Before.Customer.Services.SignIn
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Web;
    using Fanex.Alpha.Before.Common.Configurations;
    using Fanex.Alpha.Before.Common.Helpers;
    using Fanex.Alpha.Before.Customer.Helpers;
    using Fanex.Alpha.Before.Customer.Models;
    using Fanex.Cryptography.Aes;

    public static class EncryptURLHelper
    {
        public const string UTMS = "__utms";
        private const string SecretKey = "!@#$";

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
            var url = BuildAfterURL(
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
                AppSession.BrowserSize,
                AppSession.AssetAppPath);

            AppSession.ResetRobotCaptchaSession();

            return url;
        }

        public static string BuildAfterURL(
            HttpContext httpContext,
            string encryptPassword,
            VerifySignInResult verifySignInResult,
            string deviceId,
            string browserSize,
            string assetAppPath)
        {
            WriteLoginLogs(verifySignInResult.SuperId, $"Build after url, sessionId: {httpContext.Session.SessionID}, custId:{verifySignInResult.CustId}");

            var parameter = string.Format(
                "{0}&{1}&{2}&{3}&{4}&{5}&__utms&{6}&{7}&{8}&{9}&{10}&{11}&{12}",
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
                AppSession.DarkModeName,
                assetAppPath);

            var url = $"{UrlHelper.SiteGroupSetting.AfterUrl}{AppSetting.AfterLoginAuthorizationPage}?params={EncryptParameter(parameter)}";

            FPSHelper.FPSTracking(new SignInResult
            {
                Username = verifySignInResult.Username,
                ErrCode = verifySignInResult.ErrorCode,
                ErrMsg = verifySignInResult.ErrorMessage,
                LoginTime = verifySignInResult.LoginTime
            });

            return AddUMToken(httpContext, url);
        }

        private static bool NumberCrossCookie(HttpContext httpContext)
        {
            var utmsCookie = httpContext.Request.Cookies.Get(UTMS);

            return utmsCookie == null || utmsCookie.Value == AppSession.CookieCode;
        }

        private static string EncryptParameter(string parameter)
            => AesService.Default.Encrypt(parameter, SecretKey);

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
