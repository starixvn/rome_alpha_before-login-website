using Microsoft.AspNetCore.Http;

namespace Fanex.Alpha.Before.Helpers
{
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
