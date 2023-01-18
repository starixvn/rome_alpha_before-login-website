using Fanex.Alpha.Before.Helpers;

namespace Fanex.Alpha.Before.Middlewares;

public class ViewModeMiddleware
{
    private readonly RequestDelegate _next;

    public ViewModeMiddleware(RequestDelegate next)
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