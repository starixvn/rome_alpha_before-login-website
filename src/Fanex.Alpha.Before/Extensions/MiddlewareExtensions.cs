using Fanex.Alpha.Before.Middlewares;

namespace Fanex.Alpha.Before.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseRequestCulture(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestCultureMiddleware>();
    }

    public static IApplicationBuilder UseViewMode(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ViewModeMiddleware>();
    }

    public static IApplicationBuilder UseLimitedCountryAccess(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<LimitedCountryAccessMiddleware>();
    }
}