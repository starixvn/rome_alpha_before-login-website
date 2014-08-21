// -----------------------------------------------------------------------
// <copyright file="WebSettings.cs" company="Nexcel solution">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Razorbill.Sphinx.Core.Configurations
{
    using System;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using Fanex.Razorbill.Sphinx.Core.Constants;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class WebSettings
    {
        public static readonly string[] SensitiveParameters;
        private static string[] languages = new string[] { "en-US", "th-TH", "zh-CN", "zh-TW", "ja-JP", "ko-KR", "vi-VN" };

        static WebSettings()
        {
            string value = ConfigurationManager.AppSettings["SensitiveParameters"];
            if (!string.IsNullOrEmpty(value))
            {
                SensitiveParameters = value.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        public static string DefaultLanguage
        {
            get { return languages[0]; }
        }

        public static string ResourceName
        {
            get { return ConfigurationManager.AppSettings["ResourceName"]; }
        }

        public static string FpsActivator
        {
            get { return ConfigurationManager.AppSettings["FPS_Activator"]; }
        }

        public static string AppVersion
        {
            get { return ConfigurationManager.AppSettings["AppVersion"]; }
        }

        public static string DomainsExcludeDowntime
        {
            get
            {                
                return ConfigurationManager.AppSettings[CommonConstants.DOMAINSEXCLUDEDOWNTIME];
            }
        }

        public static string BaseDomain
        {
            get
            {
                return ConfigurationManager.AppSettings[CommonConstants.BASEDOMAIN];
            }
        }

        public static string DefaultLoginTemplate
        {
            get
            {
                return ConfigurationManager.AppSettings[CommonConstants.DEFAULTLOGINTEMPLATE];
            }
        }

        public static string EventLoginTemplate
        {
            get
            {
                return ConfigurationManager.AppSettings[CommonConstants.EVENTLOGINTEMPLATE];
            }
        }               

        public static string AfterLoginPage
        {
            get
            {
                return ConfigurationManager.AppSettings[CommonConstants.AFTERLOGINPAGE];
            }
        }

        /// <summary>
        /// Check is valid language.
        /// </summary>
        /// <param name="culture">User culture.</param>
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
}
