namespace Fanex.Alpha.Before.Customer.Services.FPS
{
    using Fanex.Alpha.Before.Common.Configurations;
    using Fanex.Logging.Extensions.Web;
    using global::FPS.Browserless.Detection.Client;
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.Web;

    public class RobotDetector : IRobotDetector
    {
        public int IsRobot(HttpRequest request)
        {
            if (!AppSetting.RobotEnableDetectBot)
            {
                return 0;
            }

            try
            {
                if (IsRobotAlwaysFromConfig())
                {
                    return 1;
                }

                return BotDetection.IsBot(request, AppSetting.RobotFPSBotDetectionApiUrl);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, TraceEventType.Warning);
                return 0;
            }
        }

        private static bool IsRobotAlwaysFromConfig()
        {
            return !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["Robot:IsRobotAlways"]) &&
                ConfigurationManager.AppSettings["Robot:IsRobotAlways"] == "1";
        }
    }
}
