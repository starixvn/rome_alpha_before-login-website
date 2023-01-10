using Fanex.Alpha.Before.Configurations;

namespace Fanex.Alpha.Before.Services.FPS
{
    public interface IRobotDetector
    {
        int IsRobot(HttpRequest request);
    }

    public class RobotDetector : IRobotDetector
    {
        private readonly Logging.ILogger logger;

        public RobotDetector(Logging.ILogger logger)
        {
            this.logger = logger;
        }

        public int IsRobot(HttpRequest request)
        {
            if (!AppSetting.RobotEnableDetectBot)
            {
                return 0;
            }

            try
            {
                if (AppSetting.IsRobotAlways)
                {
                    return 1;
                }

                //return BotDetection.IsBot(request, AppSetting.RobotFPSBotDetectionApiUrl);
                return 0;
            }
            catch (Exception ex)
            {
                logger.Warn(ex.Message);
                return 0;
            }
        }
    }
}
