using Fanex.Alpha.Before.Configurations;
using Fanex.Alpha.Before.Models;
using Microsoft.AspNetCore.Http.Extensions;
using ILogger = Fanex.Logging.ILogger;

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
                var fpsClient = new FPS.Client.NetStandard.Service();

                fpsClient
                    .InitialRequest(AppSetting.FpsApi, request)
                    .CheckTransaction(
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
    }
}
