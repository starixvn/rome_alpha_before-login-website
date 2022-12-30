namespace Fanex.Alpha.Before.Customer.Services.FPS
{
    using System.Web;

    public interface IRobotDetector
    {
        int IsRobot(HttpRequest request);
    }
}
