// -----------------------------------------------------------------------
// <copyright file="LanguageHelper.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Alpha.Before.Common.Helpers
{
    using System.Globalization;
    using System.Threading;
    using Fanex.Alpha.Before.Common.Configurations;
    using Fanex.Common;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class LanguageHelper
    {
        private static readonly string[] languages = { "en-US", "th-TH", "zh-CN", "zh-TW", "ja-JP", "ko-KR", "vi-VN" };

        public static string DefaultLanguage
        {
            get { return languages[0]; }
        }

        /// <summary>
        /// Checking language from cookie.
        /// </summary>
        /// <returns>Return language result.</returns>
        public static int CheckingLanguageFromCookie()
        {
            var language = 0;
            if (System.Web.HttpContext.Current.Request.Cookies["LANGUAGE"] != null)
            {
                language = Converter.ToInt(System.Web.HttpContext.Current.Request.Cookies["LANGUAGE"].Value);
            }

            if (language > 5)
            {
                language = 0;
            }

            return language;
        }

        public static void SetLanguage(string language)
        {
            AppSession.Language = language;
            Thread.CurrentThread.CurrentCulture = new CultureInfo(language);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
        }

        /// <summary>
        /// Validate language.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <returns>Return result.</returns>
        public static bool IsValidLanguage(string culture)
        {
            if (AppSetting.DisabledVietnamese && culture == "vi-VN")
            {
                return false;
            }

            int totalLangaugesSupport = languages.Length;
            for (int i = 0; i < totalLangaugesSupport; i++)
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