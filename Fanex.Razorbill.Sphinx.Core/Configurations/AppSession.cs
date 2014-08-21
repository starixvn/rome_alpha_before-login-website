// -----------------------------------------------------------------------
// <copyright file="AppSession.cs" company="Nexcel Solution">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Razorbill.Sphinx.Core.Configurations
{
    using System.Diagnostics.CodeAnalysis;
    using Fanex.Razorbill.Sphinx.Core.Constants;
    using Fanex.Razorbill.Sphinx.Core.Extensions;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class AppSession
    {
        public static string Captcha2
        {
            get
            {
                return SessionExtension.Get<string>(CommonConstants.CAPTCHA2);
            }

            set
            {
                SessionExtension.Set<string>(CommonConstants.CAPTCHA2, value);
            }
        }

        public static string Captcha3
        {
            get
            {
                return SessionExtension.Get<string>(CommonConstants.CAPTCHA3);
            }

            set
            {
                SessionExtension.Set<string>(CommonConstants.CAPTCHA3, value);
            }
        }

        public static string Username
        {
            get
            {
                return SessionExtension.Get<string>(CommonConstants.USERNAME);
            }

            set
            {
                SessionExtension.Set<string>(CommonConstants.USERNAME, value);
            }
        }

        public static string CaptchaCode
        {
            get
            {
                return SessionExtension.Get<string>(CommonConstants.CAPTCHACODE);
            }

            set
            {
                SessionExtension.Set<string>(CommonConstants.CAPTCHACODE, value);
            }
        }

        public static string Language
        {
            get
            {
                return SessionExtension.Get<string>(CommonConstants.LANGUAGE);
            }

            set
            {
                SessionExtension.Set<string>(CommonConstants.LANGUAGE, value);
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
    }
}
