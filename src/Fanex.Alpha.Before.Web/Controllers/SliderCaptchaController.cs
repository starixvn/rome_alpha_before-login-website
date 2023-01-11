using Alpha.SliderCaptcha.AspNetCore.Models;
using Alpha.SliderCaptcha.Core.Models;
using Alpha.SliderCaptcha.Core.Services;
using Fanex.Caching;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Alpha.Before.Web.Controllers;

public class SliderCaptchaController : Controller
{
    private readonly ICaptchaService captchaService;
    private readonly ICacheService cacheService;

    public SliderCaptchaController(ICaptchaService captchaService, ICacheService cacheService)
    {
        this.captchaService = captchaService;
        this.cacheService = cacheService;
    }

    [HttpPost]
    public async Task<ActionResult> Generate([FromBody] GenerateRequestModel request)
    {
        var result = await captchaService.Generate(request.Token, (CaptchaStyle)request.Style);
        return Json(result);
    }

    [HttpPost]
    public async Task<ActionResult> Verify([FromBody] VerifyRequestModel request)
    {
        var result = await captchaService.Verify(request.Token, request.Data);
        return Json(new ResponseModel(result.Code, result.Reason));
    }
}