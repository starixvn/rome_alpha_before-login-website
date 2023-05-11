using Fanex.Alpha.Before.Configurations;
using Fanex.Alpha.Before.Models;
using Fanex.Logging.Extensions.AspNetCore;
using FPS.Client.NetStandard.Models;
using FPS.Device.FingerPrint.Net.Standard.Client;
using FPS.Device.FingerPrint.Net.Standard.Client.Model;
using Microsoft.AspNetCore.Http.Extensions;
using ILogger = Fanex.Logging.ILogger;

namespace Fanex.Alpha.Before.Helpers
{
    public interface IFPSHelper
    {
        void FPSTracking(HttpRequest request, SignInResult loginResult);

        void NewFPSTracking(HttpRequest request, SignInResult loginResult);
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
                var httpRequestData = new HttpRequestModel()
                {
                    AcceptTypes = request.Headers["Accept"].ToString() ?? string.Empty,
                    UserAgent = request.Headers["User-Agent"].ToString() ?? string.Empty,
                    TotalMinutes = diff.TotalMinutes.ToString(),
                    RequestIP = ipHelper.ClientIP(),
                    Di = request.Form["__di"].ToString() ?? string.Empty,
                    Token = request.Form["__tk"].ToString() ?? string.Empty,
                    Url = request.Scheme.ToString() + "://" + request.Host.ToString(),
                    DebugInfo = request.Form["detecas-analysis"].ToString() ?? string.Empty,
                    CaptchaResult = string.Empty
                };

                new FPS.Client.NetStandard.Service(AppSetting.FpsApi).CheckTransaction(
                    httpRequestData,
                    username.Trim(),
                    loginAction,
                    loginMessage,
                    fpsSubscirber,
                    loginResult.LoginTime,
                    out var resultMessage,
                    out var deviceCode);
            }
            catch (Exception ex)
            {
                logger.Warn(ex.ToString());
            }
        }

        public void NewFPSTracking(HttpRequest request, SignInResult loginResult)
        {
            try
            {
                if (!AppSetting.NewFPSEnabled)
                {
                    return;
                }

                var fpsClient = new TransactionServices(AppSetting.FpsSubscriber, AppSetting.NewFPSPortalAPI);

                fpsClient.CheckTransaction(new TransactionModel
                {
                    Action = "Login",
                    ClientIp= ipHelper.ClientIP(),
                    CustId  = loginResult.CustId,
                    LoginName = loginResult.Username,
                    SessionKey = request.Form["__fdi"].ToString() ?? string.Empty,
                    LoginTime = loginResult.LoginTime,   
                    SubAccId = loginResult.CustSubId,
                    Url = request.GetDisplayUrl(),
                    UserAgent = request.Headers["User-Agent"].ToString() ?? string.Empty
                });
            }
            catch (Exception ex)
            {
                logger.Warn(ex.ToString());
            }
        }
    }
}
