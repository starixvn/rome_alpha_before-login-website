namespace Fanex.Alpha.Before.Customer.Services.FPS
{
    using System;
    using System.Web;
    using Fanex.Alpha.Before.Customer.Models;
    using Fanex.Logging.Extensions.Web;
    using global::FPS.Browserless.Detection.Client;
    using Newtonsoft.Json;

    public class RobotDetector : IRobotDetector
    {
        private const decimal DefaultGoogleReCaptchaScore = 1;

        public bool IsBrowserless(HttpRequest request)
        {
            try
            {
                return BrowserlessDetection.IsRobot(request);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return false;
        }

        public decimal CaptchaScore(string captchaResult)
        {
            var tokenResponse = ParseTokenResponse(captchaResult);

            return tokenResponse.Success ? tokenResponse.Score : DefaultGoogleReCaptchaScore;
        }

        private static TokenResponse ParseTokenResponse(string captchaResult)
        {
            if (!string.IsNullOrWhiteSpace(captchaResult))
            {
                try
                {
                    return JsonConvert.DeserializeObject<TokenResponse>(captchaResult);
                }
                catch (Exception ex)
                {
                    ExceptionHandler.Handle(ex);
                }
            }

            return new TokenResponse();
        }
    }
}
