namespace Fanex.Alpha.Before.Services.FPS
{
    using Fanex.Alpha.Before.Configurations;
    using System.Configuration;
    using System.Web;

    public class RobotDetector : IRobotDetector
    {
        public int IsRobot(HttpRequest request)
        {
            if (!AppSetting.RobotEnableDetectBot)
            {
                return 0;
            }

            // try
            // {
            //     if (IsRobotAlwaysFromConfig())
            //     {
            //         return 1;
            //     }

            //     return BotDetection.IsBot(request, AppSetting.RobotFPSBotDetectionApiUrl);
            // }
            // catch (Exception ex)
            // {
            //     ExceptionHandler.Handle(ex, TraceEventType.Warning);
            //     return 0;
            // }

            return 0;
        }

        private static bool IsRobotAlwaysFromConfig()
        {
            return !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["Robot:IsRobotAlways"]) &&
                ConfigurationManager.AppSettings["Robot:IsRobotAlways"] == "1";
        }
    }
}
