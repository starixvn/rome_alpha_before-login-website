namespace Fanex.Alpha.Before.Customer.Services.FPS
{
    using System;
    using System.Web;
    using Fanex.Alpha.Before.Common.Configurations;
    using Fanex.Logging.Extensions.Web;
    using global::FPS.Browserless.Core;
    using global::FPS.Browserless.Detection.Client;

    public class RobotDetector : IRobotDetector
    {
        public BotFlagged IsRobot(HttpRequest request)
        {
            if (!AppSetting.IsEnableFPS || !AppSetting.RobotEnableDetectBot)
            {
                return BotFlagged.IsClean;
            }

            try
            {
                return BotDetection.IsBot(request);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);

                return BotFlagged.IsClean;
            }
        }
    }
}
