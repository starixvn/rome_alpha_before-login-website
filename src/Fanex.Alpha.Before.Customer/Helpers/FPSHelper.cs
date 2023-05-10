using Fanex.Alpha.Before.Common.Configurations;
using Fanex.Alpha.Before.Customer.Models;
using Fanex.Logging.Extensions.Web;
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

                new FPS.Client.Service().CheckTransaction(
                    username.Trim(),
                    loginAction,
                    loginMessage,
                    fpsSubscirber,
                    loginResult.LoginTime,
                    out fpsErrorMessage,
                    out fpsErrorCode);                
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }
        }

        public static void NewFPSTracking(SignInResult loginResult)
        {
            try
            {
                if (!AppSetting.NewFPSIsEnable)
                {
                    return;
                }

                var fpsClient = new FPS.Device.FingerPrint.Net.Framework.Client.TransactionServices(AppSetting.NewFPSSubscriber, AppSetting.NewFPSPortalAPI);

                fpsClient.Insert("Login", loginResult.Username, loginResult.CustId, loginResult.CustSubId, loginResult.LoginTime);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }
        }
    }
}
