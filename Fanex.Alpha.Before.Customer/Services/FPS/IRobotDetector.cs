namespace Fanex.Alpha.Before.Customer.Services.FPS
{
    using global::FPS.Browserless.Detection.Client;
    using System.Web;

    public interface IRobotDetector
    {
        int IsRobot(HttpRequest request);
    }
}
