// -----------------------------------------------------------------------
// <copyright file="AppSettings.cs" company="">
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
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public class WebSettings
    {
        static WebSettings()
	    {
            string value = ConfigurationManager.AppSettings["SensitiveParameters"];
            if (!string.IsNullOrEmpty(value))
            {
                SensitiveParameters = value.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            }
	    }

        private static string[] languages = new string[] { "en-US", "th-TH", "zh-CN", "zh-TW", "ja-JP", "ko-KR", "vi-VN" };

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
                return ConfigurationManager.AppSettings[CommonConstants.DOMAINS_EXCLUDE_DOWNTIME];
            }
        }

        public static string BaseDomain
        {
            get
            {
                return ConfigurationManager.AppSettings[CommonConstants.BASE_DOMAIN];
            }
        }

        public static string DefaultLoginTemplate
        {
            get
            {
                return ConfigurationManager.AppSettings[CommonConstants.DEFAULT_LOGIN_TEMPLATE];
            }
        }

        public static string EventLoginTemplate
        {
            get
            {
                return ConfigurationManager.AppSettings[CommonConstants.EVENT_LOGIN_TEMPLATE];
            }
        }

        public static string SiteIBCGroup
        {
            get
            {
                return ConfigurationManager.AppSettings[CommonConstants.SITE_IBC_GROUP];
            }
        }

        public static string B8Group
        {
            get
            {
                return ConfigurationManager.AppSettings["B8Group"];
            }
        }

        public static string Site789yGroup
        {
            get
            {
                return ConfigurationManager.AppSettings[CommonConstants.SITE_SPECIAL_GROUP];
            }
        }

        public static string AfterLoginPage
        {
            get
            {
                return ConfigurationManager.AppSettings[CommonConstants.AFTER_LOGIN_PAGE];
            }
        }

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

        public readonly static string[] SensitiveParameters;
    }
}
