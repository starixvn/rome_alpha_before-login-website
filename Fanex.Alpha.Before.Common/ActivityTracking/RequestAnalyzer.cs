namespace Fanex.Alpha.Before.Common.ActivityTracking
{
    using Fanex.ActivityTracking.Web;

    public class RequestAnalyzer : BaseRequestAnalyzer
    {
        public RequestAnalyzer() : base(new JsonConverter())
        {
        }
    }
}
