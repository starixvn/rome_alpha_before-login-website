namespace Fanex.Alpha.Before.Customer.Services.FPS
{
    using Fanex.Alpha.Before.Common.Configurations;
    using Fanex.Logging.Extensions.Web;
    using global::FPS.Browserless.Detection.Client;
    using System;
    using System.Diagnostics;
    using System.Web;

    public class RobotDetector : IRobotDetector
    {
        public int IsRobot(HttpRequest request)
        {
            if (!AppSetting.IsEnableFPS || !AppSetting.RobotEnableDetectBot)
            {
                return 0;
            }

            try
            {
                return BotDetection.IsBot(request, AppSetting.RobotFPSBotDetectionApiUrl);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, TraceEventType.Warning);
                return 0;
            }
        }
    }
}
