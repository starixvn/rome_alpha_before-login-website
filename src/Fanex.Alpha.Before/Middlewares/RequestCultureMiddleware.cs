using Fanex.Alpha.Before.Helpers;

namespace Fanex.Alpha.Before.Middlewares;

public class RequestCultureMiddleware
{
    private readonly RequestDelegate _next;

    public RequestCultureMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var language = context.Request.Cookies["hidLanguage"] ?? AppSession.Language;
        LanguageHelper.SetLanguage(LanguageHelper.IsValidLanguage(language) ? language : LanguageHelper.DefaultLanguage);

        // Call the next delegate/middleware in the pipeline.
        await _next(context);
    }
}