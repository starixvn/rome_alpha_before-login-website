using System.Net;
using Fanex.Alpha.Before.Helpers;
using Fanex.Alpha.Before.Models;
using Fanex.Alpha.Before.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ONEbook.UM.Services;

namespace Fanex.Alpha.Before.Controllers;

public class CommonController : Controller
{
    private readonly IMaintenanceService _maintenanceService;
    private readonly IPHelper _ipHelper;
    private readonly ILimitedCountryAccessService _limitedCountryAccessService;
    private readonly Logging.ILogger _logger;

    public CommonController(
        IMaintenanceService maintenanceService,
        IPHelper ipHelper,
        ILimitedCountryAccessService limitedCountryAccessService,
        Logging.ILogger logger)
    {
        _maintenanceService = maintenanceService;
        _ipHelper = ipHelper;
        _limitedCountryAccessService = limitedCountryAccessService;
        _logger = logger;
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> Error(string code)
    {
        var exceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();
        if (exceptionHandlerFeature != null)
        {
            await _logger.ErrorAsync(await ExceptionHelper.BuildError(HttpContext.Request, exceptionHandlerFeature));
        }

        ViewBag.ErrorMessage = string.Format(Resources.Labels.Error_Server, string.IsNullOrWhiteSpace(code) ? (int)HttpStatusCode.InternalServerError : code);
        ViewData["DarkModeName"] = AppSession.DarkModeName;

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> Offline()
    {
        var umInfo = await _maintenanceService.GetUnderMaintenanceTimeAsync();
        if (!umInfo.IsUnderMaintenanceTime)
        {
            return Redirect("~/");
        }

        ViewData["DarkModeName"] = AppSession.DarkModeName;

        var umModel = new UnderMaintenanceModel(umInfo.IsUnderMaintenanceTime, umInfo.From, umInfo.To, umInfo.ConnectionResult.IsNotOk);
        return View(umModel);
    }

    public IActionResult LimitedCountryAccess()
    {
        var clientIP = _ipHelper.ClientIP();
        if (_limitedCountryAccessService.IsLimitedAccess(clientIP))
        {
            ViewData["DarkModeName"] = AppSession.DarkModeName;
            return View("LimitedCountryAccess", _ipHelper.ClientIP());
        }
        else
        {
            return Redirect("~/");
        }
    }
}