using Fanex.Alpha.Before.Configurations;
using Fanex.Alpha.Before.Models;
using ILogger = Fanex.Logging.ILogger;

namespace Fanex.Alpha.Before.Helpers
{
    public interface IFPSHelper
    {
        void FPSTracking(HttpRequest request, SignInResult loginResult);

        string GetDeviceID(HttpRequest request);
    }

    public class FPSHelper : IFPSHelper
    {
        private readonly ILogger logger;

        public FPSHelper(ILogger logger)
        {
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

        public string GetDeviceID(HttpRequest request)
        {
            try
            {
                if (!AppSetting.IsEnableFPS)
                {
                    return "N/A";
                }

                var fpsClient = new FPS.Client.NetStandard.Service().InitialRequest(AppSetting.FpsApi, request);
                var deviceId = fpsClient.GetDeviceID();
                return deviceId;

            }
            catch (Exception ex)
            {
                logger.Warn(ex.ToString());
                return "N/A";
            }
        }
    }
}
