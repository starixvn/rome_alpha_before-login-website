using System.Globalization;
using Fanex.Alpha.Before.Configurations;
using Fanex.Common;

namespace Fanex.Alpha.Before.Helpers
{
    public static class LanguageHelper
    {
        private static readonly string[] languages = { "en-US", "th-TH", "zh-CN", "zh-TW", "ja-JP", "ko-KR", "vi-VN" };

        public static string DefaultLanguage
        {
            get { return languages[0]; }
        }

        public static int CheckingLanguageFromCookie()
        {
            var language = 0;
            if (HttpContextHelper.Current.Request.Cookies["LANGUAGE"] != null)
            {
                language = Converter.ToInt(HttpContextHelper.Current.Request.Cookies["LANGUAGE"]);
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