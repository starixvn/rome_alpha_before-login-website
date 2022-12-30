namespace Fanex.Alpha.Before.Common.Configurations
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    public static class AppSetting
    {
        private readonly static IConfigurationRoot configurationRoot = InitConfiguration();

        private static IConfigurationRoot InitConfiguration()
        {
            const string configFileName = "appsettings.json";

            if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), configFileName)))
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile(configFileName);

                return builder.Build();
            }

            return null;
        }

        public static string Version => GetValue<string>("AppVersion");

        public static string FpsActivator => GetValue<string>("FPS_Activator");

        public static bool IsEnableFPS => !string.IsNullOrEmpty(FpsActivator) && !string.IsNullOrEmpty(configurationRoot["FPS_API"]);

        public static string GoogleAnalyticId => GetValue<string>("GoogleAnalyticId");

        public static bool IsMBCSite => GetValue<bool>("IsMBCSite");

        public static bool DisabledVietnamese => IsMBCSite;

        public static bool TrackRunTime => GetValue<bool>("TrackRunTime");

        public static bool RobotEnableDetectBot => GetValue<bool>("Robot:EnableDetectBot");

        public static IDictionary<int, bool> RobotExcludeSuperIds => ToIntDictionary(configurationRoot["Robot:ExcludeSuperIds"]);

        public static IDictionary<string, bool> RobotExcludeSites => ToDictionary(configurationRoot["Robot:ExcludeSites"]);

        public static bool IsIgnoreRobotCaptcha(int superId, string site)
            => RobotExcludeSuperIds.ContainsKey(superId)
            || RobotExcludeSites.ContainsKey(site.ToUpperInvariant());

        public static string RobotFPSBotDetectionApiUrl => GetValue<string>("Robot:FPSBotDetectionApiUrl");

        public static string TechnicalErrorTime => GetValue<string>("TechnicalErrorTime");

        public static bool IsShowTechnicalErrorMessage => !string.IsNullOrWhiteSpace(TechnicalErrorTime);

        public static string AssetAppPaths => GetValue<string>("AssetAppPaths");

        public static string AfterLoginAuthorizationPage => GetValue<string>("AfterLoginPage");

        public static Uri TelegramBotUri => new(GetValue<string>("TelegramBotUri"));

        public static bool IsCustomErrorEnabled => false;// GetValue<bool>("IsCustomErrorEnabled");

        private static T GetValue<T>(string key)
        {
            try
            {
                var value = configurationRoot[key];

                TypeConverter typeConverter = TypeDescriptor.GetConverter(typeof(T));

                return (T)typeConverter.ConvertFromString(value);
            }
            catch (Exception ex)
            {
                throw new InvalidCastException($"Key: {key}", ex);
            }
        }

        private static IDictionary<int, bool> ToIntDictionary(string value)
            => string.IsNullOrWhiteSpace(value)
            ? new Dictionary<int, bool>()
            : value
                .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .ToDictionary(kvp => int.Parse(kvp.Trim()), kvp => true);

        private static IDictionary<string, bool> ToDictionary(string value)
            => string.IsNullOrWhiteSpace(value)
            ? new Dictionary<string, bool>()
            : value
                .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .ToDictionary(kvp => kvp.Trim().ToUpperInvariant(), kvp => true);
    }
}