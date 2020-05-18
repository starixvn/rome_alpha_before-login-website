namespace Fanex.Alpha.Before.Customer.Services.FPS
{
    using System.Web;
    using global::FPS.Browserless.Core;

    public interface IRobotDetector
    {
        BotFlagged IsRobot(HttpRequest request);
    }
}
