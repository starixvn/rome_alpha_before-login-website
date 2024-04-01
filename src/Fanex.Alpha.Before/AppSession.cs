using Fanex.Alpha.Before.Constants;
using Fanex.Alpha.Before.Extensions;
using Fanex.Alpha.Before.Helpers;

namespace Fanex.Alpha.Before;

public static class AppSession
{
    private static ISession CurrentSession => HttpContextHelper.Current.Session;

    public static string Language
    {
        get
        {
            return CurrentSession.Get<string>(SessionConstants.LANGUAGE);
        }

        set
        {
            CurrentSession.Set(SessionConstants.LANGUAGE, value);
        }
    }

    public static string LangKey
    {
        get
        {
            return Language switch
            {
                "en-US" => "E",
                "ja-JP" => "J",
                "zh-TW" => "CT",// Traditional
                "zh-CN" => "CS",// Simplified
                "th-TH" => "TH",
                "ko-KR" => "KO",
                "vi-VN" => "VI",
                _ => "E",
            };
        }
    }

    public static string EncryptCode
    {
        get
        {
            return CurrentSession.Get<string>(SessionConstants.ENCRYPTCODE);
        }

        set
        {
            CurrentSession.Set<string>(SessionConstants.ENCRYPTCODE, value);
        }
    }

    public static string CaptchaCode
    {
        get
        {
            return CurrentSession.Get<string>(SessionConstants.CAPTCHACODE);
        }

        set
        {
            CurrentSession.Set<string>(SessionConstants.CAPTCHACODE, value);
        }
    }

    public static string CookieCode
    {
        get
        {
            return CurrentSession.Get<string>(SessionConstants.COOKIECODE);
        }

        set
        {
            CurrentSession.Set<string>(SessionConstants.COOKIECODE, value);
        }
    }

    public static string Username
    {
        get
        {
            return CurrentSession.Get<string>(SessionConstants.USERNAME);
        }

        set
        {
            CurrentSession.Set<string>(SessionConstants.USERNAME, value);
        }
    }

    public static string DarkModeName
    {
        get
        {
            return CurrentSession.Get<string>(SessionConstants.DARKMODENAME) ?? string.Empty;
        }

        set
        {
            CurrentSession.Set(SessionConstants.DARKMODENAME, value);
        }
    }

    #region Robot Captcha

    public static int CustId
    {
        get
        {
            return CurrentSession.Get<int>(SessionConstants.CUSTID);
        }

        set
        {
            CurrentSession.Set(SessionConstants.CUSTID, value);
        }
    }

    public static int SubAccId
    {
        get
        {
            return CurrentSession.Get<int>(SessionConstants.SUBACCID);
        }

        set
        {
            CurrentSession.Set(SessionConstants.SUBACCID, value);
        }
    }

    public static string SubAccName
    {
        get
        {
            return CurrentSession.Get<string>(SessionConstants.SUBACCNAME);
        }

        set
        {
            CurrentSession.Set(SessionConstants.SUBACCNAME, value);
        }
    }

    public static bool IsNickname
    {
        get
        {
            return CurrentSession.Get<bool>(SessionConstants.ISNICKNAME);
        }

        set
        {
            CurrentSession.Set(SessionConstants.ISNICKNAME, value);
        }
    }

    public static string Nickname
    {
        get
        {
            return CurrentSession.Get<string>(SessionConstants.NICKNAME);
        }

        set
        {
            CurrentSession.Set(SessionConstants.NICKNAME, value);
        }
    }

    public static string EncryptPassword
    {
        get
        {
            return CurrentSession.Get<string>(SessionConstants.ENCRYPTPASSWORD);
        }

        set
        {
            CurrentSession.Set(SessionConstants.ENCRYPTPASSWORD, value);
        }
    }

    public static int SuperId
    {
        get
        {
            return CurrentSession.Get<int>(SessionConstants.SUPERID);
        }

        set
        {
            CurrentSession.Set(SessionConstants.SUPERID, value);
        }
    }

    public static string Site
    {
        get
        {
            return CurrentSession.Get<string>(SessionConstants.SITE) ?? string.Empty;
        }

        set
        {
            CurrentSession.Set(SessionConstants.SITE, value);
        }
    }

    public static byte RoleId
    {
        get
        {
            return CurrentSession.Get<byte>(SessionConstants.ROLEID);
        }

        set
        {
            CurrentSession.Set(SessionConstants.ROLEID, value);
        }
    }

    public static string RobotResult
    {
        get
        {
            return CurrentSession.Get<string>(SessionConstants.ROBOTRESULT);
        }

        set
        {
            CurrentSession.Set(SessionConstants.ROBOTRESULT, value);
        }
    }

    public static int RobotCaptchaFailedCount
    {
        get
        {
            return CurrentSession.Get<int>(SessionConstants.ROBOTCAPTCHAFAILEDCOUNT);
        }

        set
        {
            CurrentSession.Set(SessionConstants.ROBOTCAPTCHAFAILEDCOUNT, value);
        }
    }

    public static string DeviceId
    {
        get
        {
            return CurrentSession.Get<string>(SessionConstants.DEVICEID);
        }

        set
        {
            CurrentSession.Set(SessionConstants.DEVICEID, value);
        }
    }

    public static string BrowserSize
    {
        get
        {
            return CurrentSession.Get<string>(SessionConstants.BROWSERSIZE);
        }

        set
        {
            CurrentSession.Set(SessionConstants.BROWSERSIZE, value);
        }
    }

    public static string AssetAppPath
    {
        get
        {
            return CurrentSession.Get<string>(SessionConstants.ASSETAPPPATH);
        }

        set
        {
            CurrentSession.Set(SessionConstants.ASSETAPPPATH, value);
        }
    }

    public static string DCSStaticAppPath
    {
        get
        {
            return CurrentSession.Get<string>(SessionConstants.DCSStaticAPPPATH);
        }

        set
        {
            CurrentSession.Set(SessionConstants.DCSStaticAPPPATH, value);
        }
    }

    public static bool InputCaptchaSuccess
    {
        get
        {
            return CurrentSession.Get<bool>(SessionConstants.INPUTCAPTCHASUCCESS);
        }

        set
        {
            CurrentSession.Set(SessionConstants.INPUTCAPTCHASUCCESS, value);
        }
    }


    public static void AssignRobotCaptchaSession(
        int custId,
        string username,
        int subAccId,
        string subAccName,
        bool isNickname,
        string nickname,
        string encryptPassword,
        int superId,
        string site,
        byte roleId,
        string robotResult,
        string deviceId,
        string browserSize,
        string assetAppPath,
        string dcsStaticAppPath)
    {
        CustId = custId;
        Username = username;
        SubAccId = subAccId;
        SubAccName = subAccName;
        IsNickname = isNickname;
        Nickname = nickname;
        EncryptPassword = encryptPassword;
        SuperId = superId;
        Site = site;
        RoleId = roleId;

        RobotResult = robotResult;
        DeviceId = deviceId;
        BrowserSize = browserSize;
        AssetAppPath = assetAppPath;
        DCSStaticAppPath = dcsStaticAppPath;
    }

    public static void ResetRobotCaptchaSession()
    {
        CustId = 0;
        Username = null;
        SubAccId = 0;
        SubAccName = null;
        IsNickname = false;
        Nickname = null;
        EncryptPassword = null;
        SuperId = 0;
        Site = null;
        RoleId = 0;

        RobotResult = null;
        RobotCaptchaFailedCount = 0;
        DeviceId = null;
        BrowserSize = null;
        InputCaptchaSuccess = false;
    }

    public static void IncreaseRobotCaptchaFailedCount()
    {
        if (RobotCaptchaFailedCount < int.MaxValue)
        {
            RobotCaptchaFailedCount++;
        }
    }

    #endregion
}
