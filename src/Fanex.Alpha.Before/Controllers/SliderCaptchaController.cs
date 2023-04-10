using Alpha.Captcha.Core.Models;
using Alpha.Captcha.Core.Services;
using Alpha.SliderCaptcha.AspNetCore.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Alpha.Before.Controllers;

public class SliderCaptchaController : Controller
{
    private readonly ICaptchaService captchaService;
    private readonly Logging.ILogger logger;

    public SliderCaptchaController(ICaptchaService captchaService, Logging.ILogger logger)
    {
        this.captchaService = captchaService;
        this.logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult> Generate([FromBody] GenerateRequestModel request)
    {
        try
        {
            var result = await captchaService.Generate(request.Token, (CaptchaStyle)request.Style);
            return Json(result);
        }
        catch (Exception ex)
        {
            await logger.WarnAsync(ex.ToString());
            return Json(new GenerateResult(string.Empty, null));
        }
    }

    [HttpPost]
    public async Task<ActionResult> Verify([FromBody] VerifyRequestModel request)
    {
        try
        {
            var result = await captchaService.Verify(request.Token, request.Data);
            return Json(new ResponseModel(result.Code, result.Reason));
        }
        catch (Exception ex)
        {
            await logger.WarnAsync(ex.ToString());
            return Json(new ResponseModel(ResponseModel.SuccessCode, string.Empty));
        }
    }
}