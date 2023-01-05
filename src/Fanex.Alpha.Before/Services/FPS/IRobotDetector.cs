namespace Fanex.Alpha.Before.Services.FPS
{
    using System.Web;

    public interface IRobotDetector
    {
        int IsRobot(HttpRequest request);
    }
}
