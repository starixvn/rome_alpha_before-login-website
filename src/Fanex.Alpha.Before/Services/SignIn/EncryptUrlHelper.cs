using Fanex.Alpha.Before.Configurations;
using Fanex.Alpha.Before.Models;
using Fanex.Cryptography.Aes;

namespace Fanex.Alpha.Before.Services.SignIn
{
    public interface IEncryptUrlHelper
    {
        void WriteLoginLogs(int superId, string message);

        string BuildAfterURL(HttpContext httpContext);

        string BuildAfterURL(
            HttpContext httpContext,
            string encryptPassword,
            VerifySignInResult verifySignInResult,
            string language,
            string deviceId,
            string browserSize,
            string encryptCode,
            string darkModeName,
            string assetAppPath,
             string dcsStaticAppPath);
    }

    public class EncryptUrlHelper : IEncryptUrlHelper
    {
        public const string UTMS = "__utms";
        private const string SecretKey = "!@#$";

        public void WriteLoginLogs(int superId, string message)
        {
            var superIds = AppSetting.GetValue<int[]>("Robot:TrackLogSuperIds") ?? Array.Empty<int>();
            if (superIds.Any(id => id == superId))
            {
                Logging.Logger.Log.Warn(message);
            }
        }

        public string BuildAfterURL(HttpContext httpContext)
        {
            WriteLoginLogs(AppSession.SuperId, $"After input captcha, sessionId: {httpContext.Session.Id}, custId:{AppSession.CustId}");
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
                AppSession.Language,
                AppSession.DeviceId,
                AppSession.BrowserSize,
                AppSession.EncryptCode,
                AppSession.DarkModeName,
                AppSession.AssetAppPath,
                AppSession.DCSStaticAppPath);

            AppSession.ResetRobotCaptchaSession();

            return url;
        }

        public string BuildAfterURL(
            HttpContext httpContext,
            string encryptPassword,
            VerifySignInResult verifySignInResult,
            string language,
            string deviceId,
            string browserSize,
            string encryptCode,
            string darkModeName,
            string assetAppPath,
            string dcsStaticAppPath)
        {
            WriteLoginLogs(verifySignInResult.SuperId, $"Build after url, sessionId: {httpContext.Session.Id}, custId:{verifySignInResult.CustId}");

            var parameter = string.Format(
                "{0}&{1}&{2}&{3}&{4}&{5}&__utms&{6}&{7}&{8}&{9}&{10}&{11}&{12}&{13}",
                verifySignInResult.CustId,
                verifySignInResult.SubAccId,
                verifySignInResult.LoginTime.Ticks,
                encryptPassword,
                verifySignInResult.Nickname,
                verifySignInResult.IsNickname,
                language,
                NumberCrossCookie(httpContext),
                deviceId,
                browserSize,
                encryptCode,
                darkModeName,
                assetAppPath,
                dcsStaticAppPath);

            var host = httpContext.Request.Host.Value.TrimEnd('.');
            var afterUrl = AppSetting.DomainMappings[host];

            var url = $"{afterUrl}{AppSetting.AfterLoginAuthorizationPage}?params={EncryptParameter(parameter)}";

            return AddUMToken(httpContext, url);
        }

        private static bool NumberCrossCookie(HttpContext httpContext)
        {
            var utmsCookie = httpContext.Request.Cookies[UTMS];

            return string.IsNullOrWhiteSpace(utmsCookie) || utmsCookie == AppSession.CookieCode;
        }

        private static string EncryptParameter(string parameter)
            => AesService.Default.Encrypt(parameter, SecretKey);

        private static string AddUMToken(HttpContext httpContext, string url)
        {
            var umToken = httpContext.Session.GetString("UMToken");

            return umToken == null
                ? url
                : $"{url}&UMToken={umToken}";
        }
    }
}
