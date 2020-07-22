namespace Fanex.Alpha.Before.Customer.ThrottlingFilters
{
    using global::Alpha.NetThrottling.Filters;
    using global::Alpha.NetThrottling.Utils;
    using System.Web;

    public class LoginFilter : IFilter
    {
        public string GetKey(HttpContext httpContext)
        {
            var request = httpContext.Request;
            var clientIp = request.GetClientIp().ToString();

            var username = request.Form["txtUserName"];
            if (string.IsNullOrWhiteSpace(username))
            {
                return null;
            }

            username = username.Trim().ToLowerInvariant();
            return $"{clientIp}_{username}";
        }
    }
}
