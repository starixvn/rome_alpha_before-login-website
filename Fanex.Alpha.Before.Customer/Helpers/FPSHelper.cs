using Fanex.Alpha.Before.Common.Configurations;
using Fanex.Alpha.Before.Customer.Models;
using Fanex.Logging.Extensions.Web;
using System;
using System.Configuration;

namespace Fanex.Alpha.Before.Customer.Helpers
{
    public static class FPSHelper
    {
        public static void FPSTracking(SignInResult loginResult, DateTime loginTime)
        {
            try
            {
                if (!AppSetting.IsEnableFPS)
                {
                    return;
                }

                const string loginAction = "login";
                var loginMessage = string.Empty;
                var username = loginResult?.Username;

                var fpsSubscirber = ConfigurationManager.AppSettings["FPS_Subscriber"];
                var fpsErrorMessage = string.Empty;
                var fpsErrorCode = string.Empty;

                if (loginResult == null)
                {
                    loginMessage = "failed";
                }
                else if (loginResult.ErrCode != -1)
                {
                    loginMessage = string.Format("{0}", loginResult.ErrMsg);
                }
                else
                {
                    loginMessage = "successfully";
                }

                new FPS.Client.Service().CheckTransaction(
                    username?.Trim(),
                    loginAction,
                    loginMessage,
                    fpsSubscirber,
                    loginTime,
                    out fpsErrorMessage,
                    out fpsErrorCode);                
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }
        }
    }
}
