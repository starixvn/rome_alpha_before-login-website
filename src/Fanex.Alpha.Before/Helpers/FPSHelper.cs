using Fanex.Alpha.Before.Configurations;
using Fanex.Alpha.Before.Models;
using Fanex.Logging.Extensions.AspNetCore;
using FPS.Client.NetStandard.Models;
using FPS.Device.FingerPrint.Net.Standard.Client;
using FPS.Device.FingerPrint.Net.Standard.Client.Model;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;
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
                var fpsClient = new FPS.Client.NetStandard.Service(AppSetting.FpsApi, request);

                fpsClient.CheckTransaction(
                    loginResult.Username,
                    AppSetting.FpsSubscriber,
                    "login",                   
                    loginResult.LoginTime);

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

                var data = new TransactionModel
                {
                    Action = "Login",
                    ClientIp = ipHelper.ClientIP(),
                    CustId = loginResult.CustId,
                    LoginName = loginResult.Username,
                    SessionKey = request.Form["__fdi"].ToString() ?? string.Empty,
                    LoginTime = loginResult.LoginTime,
                    SubAccId = loginResult.CustSubId,
                    Url = request.GetDisplayUrl(),
                    UserAgent = request.Headers["User-Agent"].ToString() ?? string.Empty
                };

                fpsClient.CheckTransaction(data);
            }
            catch (Exception ex)
            {
                logger.Warn(ex.ToString());
            }
        }
    }
}
