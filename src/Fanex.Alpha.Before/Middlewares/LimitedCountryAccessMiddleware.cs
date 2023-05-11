using Fanex.Alpha.Before.Helpers;
using Fanex.Alpha.Before.Services;
using Microsoft.AspNetCore.Authorization;
using ONEbook.UM.Services;

namespace Fanex.Alpha.Before.Middlewares;

public class LimitedCountryAccessMiddleware
{
    private const string LimitedCountryAccessPath = "/Common/LimitedCountryAccess";
    private readonly RequestDelegate _next;
    private readonly IPHelper _ipHelper;
    private readonly ILimitedCountryAccessService _limitedCountryAccessService;
    private readonly IMaintenanceService _maintenanceService;


    public LimitedCountryAccessMiddleware(
        RequestDelegate next,
        IPHelper ipHelper,
        ILimitedCountryAccessService limitedCountryAccessService,
        IMaintenanceService maintenanceService)
    {
        _next = next;
        _ipHelper = ipHelper;
        _limitedCountryAccessService = limitedCountryAccessService;
        _maintenanceService = maintenanceService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() is object)
        {
            await _next(context);
            return;
        }

        if (await _maintenanceService.IsUnderMaintenanceAsync())
        {
            await _next(context);
            return;
        }

        if (_limitedCountryAccessService.IsLimitedAccess(_ipHelper.ClientIP()) && context.Request.Path.Value != LimitedCountryAccessPath)
        {
            context.Response.Redirect(LimitedCountryAccessPath, permanent: true, preserveMethod: true);
            return;
        }

        // Call the next delegate/middleware in the pipeline.
        await _next(context);
    }
}