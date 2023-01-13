using Fanex.Alpha.Before.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Fanex.Alpha.Before.Services.FPS
{
    public interface IRobotDetector
    {
        int IsRobot(HttpRequest request);
    }

    public class RobotDetector : IRobotDetector
    {
        private readonly Logging.ILogger logger;
        private readonly RobotOptions robotOptions;

        public RobotDetector(Logging.ILogger logger, IOptions<RobotOptions> robotOptions)
        {
            this.logger = logger;
            this.robotOptions = robotOptions.Value;
        }

        public int IsRobot(HttpRequest request)
        {
            if (!robotOptions.EnableDetectBot)
            {
                return 0;
            }

            try
            {
                if (robotOptions.IsRobotAlways)
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
