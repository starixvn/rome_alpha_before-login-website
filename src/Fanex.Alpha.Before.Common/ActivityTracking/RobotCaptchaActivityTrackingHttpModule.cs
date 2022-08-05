namespace Fanex.Alpha.Before.Common.ActivityTracking
{
    using Fanex.ActivityTracking.Web;

    public class RobotCaptchaActivityTrackingHttpModule : ActivityTrackingHttpModule
    {
        public RobotCaptchaActivityTrackingHttpModule()
             : base(new RobotCaptchaSession(), new RequestAnalyzer(), new JsonConverter())
        {
        }
    }
}
