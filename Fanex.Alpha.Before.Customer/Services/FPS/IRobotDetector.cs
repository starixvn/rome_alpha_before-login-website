namespace Fanex.Alpha.Before.Customer.Services.FPS
{
    using global::FPS.Browserless.Detection.Client;
    using System.Web;

    public interface IRobotDetector
    {
        BotFlagged IsRobot(HttpRequest request);
    }
}
