using Fanex.Alpha.Before.Configurations;
using Fanex.Alpha.Before.Helpers;
using FPS.Browserless.Detection.Client.NetStandard;
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
        private readonly IPHelper ipHelper;
        private readonly IPService ipService;

        public RobotDetector(
            Logging.ILogger logger,
            IOptions<RobotOptions> robotOptions,
            IPHelper ipHelper,
            IPService ipService)
        {
            this.logger = logger;
            this.robotOptions = robotOptions.Value;
            this.ipHelper = ipHelper;
            this.ipService = ipService;
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

                var ipInfo = ipService.GetIPInformation(ipHelper.ClientIP());
                if (ipInfo != null && ipInfo.CountryShort == "MY")
                {
                    return 0;
                }

                return BotDetection.IsBot(request, robotOptions.FPSBotDetectionApiUrl);
            }
            catch (Exception ex)
            {
                logger.Warn(ex.Message);
                return 0;
            }
        }
    }
}
