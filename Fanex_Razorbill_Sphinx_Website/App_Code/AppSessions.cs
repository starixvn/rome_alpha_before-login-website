// --------------------------------------------------------------------------
// <copyright file="AppSessions.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Jeff.Nguyen" date="2012.06.04">Improve performance, check StypeCop</change>
// </history>

using System;
using System.Web;

/// <summary>
/// TODO: App sessions.
/// </summary>
public static class AppSessions
{
    private static string[] languages = new string[] { "en-US", "th-TH", "zh-CN", "zh-TW", "ja-JP", "ko-KR", "vi-VN" };

    public static string Language
    {
        get
        {
            return HttpContext.Current.Session["Language"] as string;
        }

        set
        {
            for (int i = 0; i < languages.Length; ++i)
            {
                if (value == languages[i])
                {
                    HttpContext.Current.Session["Language"] = value;
                    break;
                }
            }
        }
    }

    public static string UserName
    {
        get { return HttpContext.Current.Session["UserName"] as string; }
        set { HttpContext.Current.Session["UserName"] = value; }
    }

    public static string CaptchaCode
    {
        get
        {
            return HttpContext.Current.Session["CaptchaCode"] as string;
        }

        set
        {
            HttpContext.Current.Session["CaptchaCode"] = value;
        }
    }

    public static string LangKey
    {
        get
        {
            switch (AppSessions.Language)
            {
                case "en-US": return "E";
                case "ja-JP": return "J";
                case "zh-TW": return "CT"; // Traditional
                case "zh-CN": return "CS"; // Simplified
                case "th-TH": return "TH";
                case "ko-KR": return "KO";
                case "vi-VN": return "VI";
            }

            return "E";
        }
    }

    public static int LoginFailedCount
    {
        get
        {
            if (HttpContext.Current.Session["LoginFailedCount"] == null) 
            { 
                return 0; 
            }

            return (int)HttpContext.Current.Session["LoginFailedCount"];
        }

        set
        {
            HttpContext.Current.Session["LoginFailedCount"] = value;
        }
    }

    public static DateTime PreviousLoginFailedTime
    {
        get
        {
            if (HttpContext.Current.Session["PreviousLoginFailedTime"] == null)
            { 
                HttpContext.Current.Session["PreviousLoginFailedTime"] = DateTime.MinValue; 
            }

            return Convert.ToDateTime(HttpContext.Current.Session["PreviousLoginFailedTime"]);
        }

        set
        {
            HttpContext.Current.Session["PreviousLoginFailedTime"] = value;
        }
    }

    /// <summary>
    /// Check is valid language.
    /// </summary>
    /// <param name="culture">Country parameter.</param>
    /// <returns>Return result.</returns>
    public static bool IsValidLanguage(string culture)
    {
        int c = languages.Length;
        for (int i = 0; i < c; i++)
        {
            if (languages[i] == culture)
            {
                return true;
            }
        }

        return false;
    }
}
