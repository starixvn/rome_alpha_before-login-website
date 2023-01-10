using Alpha.SliderCaptcha.AspNetCore.Models;
using Alpha.SliderCaptcha.Core.Models;
using Alpha.SliderCaptcha.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Alpha.Before.Web.Controllers;

public class SliderCaptchaController : Controller
{
    private readonly ICaptchaService captchaService;

    public SliderCaptchaController(ICaptchaService captchaService)
    {
        this.captchaService = captchaService;
    }

    [HttpPost]
    public async Task<ActionResult> Generate([FromBody] GenerateRequestModel request)
    {
        var result = await captchaService.GenerateAsync(request.Token, (CaptchaStyle)request.Style).ConfigureAwait(false);
        return Json(result);
    }

    [HttpPost]
    public async Task<ActionResult> Verify([FromBody] VerifyRequestModel request)
    {
        var result = await captchaService.VerifyAsync(request.Token, request.Data).ConfigureAwait(false);
        return Json(new ResponseModel(result.Code, result.Reason));
    }
}