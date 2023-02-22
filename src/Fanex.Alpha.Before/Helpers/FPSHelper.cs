using Fanex.Alpha.Before.Configurations;
using Fanex.Alpha.Before.Models;
using FPS.Client.NetStandard.Models;
using ILogger = Fanex.Logging.ILogger;
using System.Text.Json;

namespace Fanex.Alpha.Before.Helpers
{
    public interface IFPSHelper
    {
        void FPSTracking(HttpRequest request, SignInResult loginResult);
    }

    public class FPSHelper : IFPSHelper
    {
        private readonly IPHelper ipHelper;
        private readonly ILogger logger;

        public FPSHelper(IPHelper ipHelper, ILogger logger)
        {
            this.ipHelper = ipHelper;
            this.logger = logger;
        }

        public void FPSTracking(HttpRequest request, SignInResult loginResult)
        {
            try
            {
                if (!AppSetting.IsEnableFPS)
                {
                    return;
                }

                var loginMessage = string.Empty;
                if (loginResult == null)
                {
                    loginResult = new SignInResult();
                    loginMessage = "failed";
                }
                else if (loginResult.ErrCode != -1)
                {
                    loginMessage = string.Format("{0}", loginResult.ErrMsg);
                }
                else
                {
                    loginMessage = "successfully";
                }

                const string loginAction = "login";
                var username = loginResult.Username;
                var fpsSubscirber = AppSetting.FpsSubscriber;
                TimeSpan diff = DateTime.UtcNow - DateTime.Now;
                var httpRequestData = new HttpRequestData()
                {
                    AcceptTypes = request.Headers["Accept"].ToString() ?? string.Empty,
                    UserAgent = request.Headers["User-Agent"].ToString() ?? string.Empty,
                    TotalMinutes = diff.TotalMinutes.ToString(),
                    RequestIP = ipHelper.ClientIP(),
                    Di = request.Form["__di"].ToString(),
                    Token = request.Form["__tk"].ToString(),
                    Url = request.Scheme.ToString() + "://" + request.Host.ToString(),
                    DebugInfo = request.Form["detecas-analysis"].ToString(),
                    CaptchaResult = string.Empty
                };

                logger.Info(JsonSerializer.Serialize(httpRequestData));

                new FPS.Client.NetStandard.Service(AppSetting.FpsApi).CheckTransaction(
                    httpRequestData,
                    username.Trim(),
                    loginAction,
                    loginMessage,
                    fpsSubscirber,
                    loginResult.LoginTime);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
        }
    }
}
