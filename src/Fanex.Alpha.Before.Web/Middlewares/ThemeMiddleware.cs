using Fanex.Alpha.Before.Common.Helpers;

namespace Fanex.Alpha.Before.Web.Middlewares;

public class ThemeMiddleware
{
    private readonly RequestDelegate _next;

    public ThemeMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        DarkModeHelper.SetDarkModeSession(context.Request);

        // Call the next delegate/middleware in the pipeline.
        await _next(context);
    }
}