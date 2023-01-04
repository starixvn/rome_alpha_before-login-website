namespace Fanex.Alpha.Before.Common.Helpers
{
    using System.Web;
    using Fanex.Alpha.Before.Common.Configurations;

    public static class DarkModeHelper
    {
        public static void SetDarkModeSession(HttpRequest request)
            => AppSession.DarkModeName = GetCookieIsDarkModeValue(request) ? "-dark" : string.Empty;

        private static bool GetCookieIsDarkModeValue(HttpRequest request)
        {
            var cookie = request.Cookies["ISDARKMODE"];

            return "1" == cookie;
        }
    }
}
