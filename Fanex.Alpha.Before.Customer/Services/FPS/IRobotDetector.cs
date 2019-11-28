namespace Fanex.Alpha.Before.Customer.Services.FPS
{
    using System.Web;

    public interface IRobotDetector
    {
        bool IsBrowserless(HttpRequest request);

        decimal CaptchaScore(string captchaResult);
    }
}
