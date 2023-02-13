using Fanex.Logging;
using Microsoft.AspNetCore.HttpOverrides;

namespace Fanex.Alpha.Before.Helpers;

public interface IPHelper
{
    string ClientIP();
}

public class IPHelperImpl : IPHelper
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public IPHelperImpl(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string ClientIP()
    {
        var context = _httpContextAccessor.HttpContext;
        var ip = context.Request.Headers["X-Forwarded-For"].ToString() ?? context.Connection.RemoteIpAddress.ToString();

        if (string.IsNullOrWhiteSpace(ip))
        {
            var ex = new Exception("Before login: ip is null or empty");
            Logger.Log.Error(ex.Message, ex);
        }

        var ips = string.IsNullOrWhiteSpace(ip) || ip.Equals("::1") ?
            new[] { "127.0.0.1" } : ip.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        if (string.IsNullOrWhiteSpace(ips[0]))
        {
            var ex = new Exception("Before login: HTTP_X_FORWARDED_FOR ip is null or empty");
            Logger.Log.Error(ex.Message, ex);
        }

        return ips[0].Trim();
    }
}

