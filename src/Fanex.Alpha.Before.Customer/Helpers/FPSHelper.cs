using Fanex.Alpha.Before.Common.Configurations;
using Fanex.Alpha.Before.Customer.Models;
using Fanex.Logging;
using System;
using System.Configuration;

namespace Fanex.Alpha.Before.Customer.Helpers
{
    public static class FPSHelper
    {
        public static void FPSTracking(SignInResult loginResult)
        {
            try
            {
                if (!AppSetting.IsEnableFPS)
                {
                    return;
                }


                var loginMessage = string.Empty;
                if (loginResult == null)
                {
                    loginResult = new SignInResult();
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

                const string loginAction = "login";
                var username = loginResult.Username;
                var fpsSubscirber = ConfigurationManager.AppSettings["FPS_Subscriber"];
                var fpsErrorMessage = string.Empty;
                var fpsErrorCode = string.Empty;

                // new FPS.Client.Service().CheckTransaction(
                //     username.Trim(),
                //     loginAction,
                //     loginMessage,
                //     fpsSubscirber,
                //     loginResult.LoginTime,
                //     out fpsErrorMessage,
                //     out fpsErrorCode);                
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.Message, ex);
            }
        }
    }
}
