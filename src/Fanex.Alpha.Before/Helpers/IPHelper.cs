using ILogger = Fanex.Logging.ILogger;

namespace Fanex.Alpha.Before.Helpers;

public interface IPHelper
{
    string ClientIP();
}

public class IPHelperImpl : IPHelper
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger _logger;

    public IPHelperImpl(IHttpContextAccessor httpContextAccessor, ILogger logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public string ClientIP()
    {
        var context = _httpContextAccessor.HttpContext;
        var xActualIP = context.Request.Headers["X-Actual-IP"].ToString();
        var xForwardedFor = !string.IsNullOrWhiteSpace(xActualIP) ? xActualIP : context.Request.Headers["X-Forwarded-For"].ToString();
        var ip = !string.IsNullOrEmpty(xForwardedFor) ? xForwardedFor : context.Connection.RemoteIpAddress.ToString();

        if (string.IsNullOrWhiteSpace(ip))
        {
            var ex = new Exception("Before login: ip is null or empty");
            _logger.Error(ex.Message, ex);
        }

        var ips = string.IsNullOrWhiteSpace(ip) || ip.Equals("::1") ?
            new[] { "127.0.0.1" } : ip.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        if (string.IsNullOrWhiteSpace(ips[0]))
        {
            var ex = new Exception("Before login: HTTP_X_FORWARDED_FOR ip is null or empty");
            _logger.Error(ex.Message, ex);
        }

        return ips[0].Trim();
    }
}

