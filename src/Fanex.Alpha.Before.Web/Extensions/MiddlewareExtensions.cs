using Fanex.Alpha.Before.Web.Middlewares;

namespace Fanex.Alpha.Before.Web.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseRequestCulture(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestCultureMiddleware>();
    }

    public static IApplicationBuilder UseTheme(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ThemeMiddleware>();
    }

    public static IApplicationBuilder UseLimitedCountryAccess(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<LimitedCountryAccessMiddleware>();
    }
}