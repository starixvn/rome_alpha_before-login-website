using System.Net;
using Fanex.Alpha.Before.Helpers;
using Fanex.Alpha.Before.Models;
using Fanex.Logging.Extensions.AspNetCore;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ONEbook.UM.Services;

namespace Fanex.Alpha.Before.Web.Controllers;

public class CommonController : Controller
{
    private readonly IMaintenanceService _maintenanceService;
    private readonly IPHelper _ipHelper;

    public CommonController(IMaintenanceService maintenanceService, IPHelper ipHelper)
    {
        _maintenanceService = maintenanceService;
        _ipHelper = ipHelper;
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> Error()
    {
        var error = HttpContext.Features.Get<IExceptionHandlerFeature>().Error;
        await ExceptionHandler.HandleAsync(error, HttpContext);

        ViewBag.ErrorMessage = string.Format(Resources.Labels.Error_Server, HttpStatusCode.InternalServerError);
        ViewData["DarkModeName"] = AppSession.DarkModeName;

        return View();
    }

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
        ViewData["DarkModeName"] = AppSession.DarkModeName;
        return View("LimitedCountryAccess", _ipHelper.ClientIP());
    }
}