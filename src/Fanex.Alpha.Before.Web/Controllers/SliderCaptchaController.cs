using Alpha.Captcha.Core.Models;
using Alpha.Captcha.Core.Services;
using Alpha.SliderCaptcha.AspNetCore.Models;
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