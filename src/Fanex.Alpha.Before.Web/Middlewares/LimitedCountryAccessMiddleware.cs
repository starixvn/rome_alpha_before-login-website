using Fanex.Alpha.Before.Helpers;
using Fanex.Alpha.Before.Services;

namespace Fanex.Alpha.Before.Web.Middlewares;

public class LimitedCountryAccessMiddleware
{
    private const string LimitedCountryAccessPath = "/Common/LimitedCountryAccess";
    private readonly RequestDelegate _next;
    private readonly IPHelper _ipHelper;
    private readonly ILimitedCountryAccessService _limitedCountryAccessService;


    public LimitedCountryAccessMiddleware(
        RequestDelegate next,
        IPHelper ipHelper,
        ILimitedCountryAccessService limitedCountryAccessService)
    {
        _next = next;
        _ipHelper = ipHelper;
        _limitedCountryAccessService = limitedCountryAccessService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (_limitedCountryAccessService.IsLimitedAccess(_ipHelper.ClientIP()) && context.Request.Path.Value != LimitedCountryAccessPath)
        {
            context.Response.Redirect(LimitedCountryAccessPath, permanent: true, preserveMethod: true);
            return;
        }

        // Call the next delegate/middleware in the pipeline.
        await _next(context);
    }
}