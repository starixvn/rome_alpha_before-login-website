<%@ WebHandler Language="C#" Class="LoginCaptcha" %>

using System;
using System.Web;
using System.Web.SessionState;
using Sunplus.Agent.Utilities;
using Fanex.Age.Utility.CaptchaUtil;

public class LoginCaptcha : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        context.Response.Cache.SetNoStore();

        #region Captcha
        string ip = context.Request.UserHostAddress;
        var captchaUtil = new CaptchaImage(ip, AppSessions.UserName, CaptchaType.Text);
        string code = "";
        string value = "";
        var bitmap = captchaUtil.Generate(out code, out value);
        AppSessions.CaptchaCode = value;
        //AppSessions.CaptchaType = captchaUtil.Choice;

        // Set the image.
        // Change the response headers to output a JPEG image.
        context.Response.Clear();
        context.Response.ContentType = "image/Jpeg";

        // Write the image to the response stream in JPEG format.
        bitmap.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg);

        // Dispose of the CAPTCHA image object.
        bitmap.Dispose();
        #endregion
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}