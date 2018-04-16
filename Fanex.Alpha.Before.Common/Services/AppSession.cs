// -----------------------------------------------------------------------
// <copyright file="AppSession.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Alpha.Before.Common.Configurations
{
    using Fanex.Alpha.Before.Common.Constants;
    using Fanex.Alpha.Before.Infrastructure.Extensions;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class AppSession
    {
        public static string Language
        {
            get
            {
                return SessionExtension.Get<string>(SessionConstants.LANGUAGE);
            }

            set
            {
                SessionExtension.Set<string>(SessionConstants.LANGUAGE, value);
            }
        }

        public static string LangKey
        {
            get
            {
                switch (AppSession.Language)
                {
                    case "en-US": return "E";
                    case "ja-JP": return "J";
                    case "zh-TW": return "CT"; // Traditional
                    case "zh-CN": return "CS"; // Simplified
                    case "th-TH": return "TH";
                    case "ko-KR": return "KO";
                    case "vi-VN": return "VI";
                    default: return "E";
                }
            }
        }

        public static string EncryptCode
        {
            get
            {
                return SessionExtension.Get<string>(SessionConstants.ENCRYPTCODE);
            }

            set
            {
                SessionExtension.Set<string>(SessionConstants.ENCRYPTCODE, value);
            }
        }

        public static string CaptchaCode
        {
            get
            {
                return SessionExtension.Get<string>(SessionConstants.CAPTCHACODE);
            }

            set
            {
                SessionExtension.Set<string>(SessionConstants.CAPTCHACODE, value);
            }
        }

        public static string CookieCode
        {
            get
            {
                return SessionExtension.Get<string>(SessionConstants.COOKIECODE);
            }

            set
            {
                SessionExtension.Set<string>(SessionConstants.COOKIECODE, value);
            }
        }

        public static string Username
        {
            get
            {
                return SessionExtension.Get<string>(SessionConstants.USERNAME);
            }

            set
            {
                SessionExtension.Set<string>(SessionConstants.USERNAME, value);
            }
        }
    }
}
