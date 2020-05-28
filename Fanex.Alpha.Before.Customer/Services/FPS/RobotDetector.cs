namespace Fanex.Alpha.Before.Customer.Services.FPS
{
    using System;
    using System.Web;
    using Fanex.Logging.Extensions.Web;
    using global::FPS.Browserless.Core;
    using global::FPS.Browserless.Detection.Client;

    public class RobotDetector : IRobotDetector
    {
        public BotFlagged IsRobot(HttpRequest request)
        {
            try
            {
                return BotDetection.IsBot(request);
            }
#pragma warning disable CS0168 // Variable is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // Variable is declared but never used
            {
                
#pragma warning disable S125 // Sections of code should not be commented out
// ExceptionHandler.Handle(ex);

                return BotFlagged.IsClean;
#pragma warning restore S125 // Sections of code should not be commented out
            }
        }
    }
}
