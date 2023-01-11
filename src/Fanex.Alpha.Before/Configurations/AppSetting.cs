namespace Fanex.Alpha.Before.Configurations
{
    public static class AppSetting
    {
        private const string Site789y = "789y";
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

        public static string SiteGroup => GetValue<string>("SiteGroup");

        public static bool Is789ySite => SiteGroup == Site789y;

        public static bool DisabledVietnamese => !Is789ySite;

        public static IDictionary<string, string> DomainMappings
            => configurationRoot.GetSection("DomainMappings").Get<DomainMapping[]>().ToDictionary(kvp => kvp.Before, kvp => kvp.After);

        public static string Version => GetValue<string>("AppVersion");

        public static string FpsActivator => GetValue<string>("FPS:Activator");

        public static string FpsSubscriber => GetValue<string>("FPS:Subscriber");

        public static bool IsEnableFPS => !string.IsNullOrEmpty(FpsActivator) && !string.IsNullOrEmpty(configurationRoot["FPS:API"]);

        public static string GoogleAnalyticId => GetValue<string>("GoogleAnalyticId");

        public static bool TrackRunTime => GetValue<bool>("TrackRunTime");

        public static bool RobotEnableDetectBot => GetValue<bool>("Robot:EnableDetectBot");

        public static bool IsRobotAlways => GetValue<bool>("Robot:IsRobotAlways");

        public static int[] RobotExcludeSuperIds => configurationRoot.GetValue("Robot:ExcludeSuperIds", Array.Empty<int>());

        public static string[] RobotExcludeSites => configurationRoot.GetValue("Robot:ExcludeSites", Array.Empty<string>());

        public static bool IsIgnoreRobotCaptcha(int superId, string site)
            => RobotExcludeSuperIds.Contains(superId)
            || RobotExcludeSites.Contains(site.ToUpperInvariant());

        public static string RobotFPSBotDetectionApiUrl => GetValue<string>("Robot:FPSBotDetectionApiUrl");

        public static string TechnicalErrorTime => GetValue<string>("TechnicalErrorTime");

        public static bool IsShowTechnicalErrorMessage => !string.IsNullOrWhiteSpace(TechnicalErrorTime);

        public static string AssetAppPaths => GetValue<string>("AssetAppPaths");

        public static string AfterLoginAuthorizationPage => GetValue<string>("AfterLoginPage");

        public static T GetValue<T>(string key)
        {
            try
            {
                return configurationRoot.GetValue<T>(key);
            }
            catch (Exception ex)
            {
                throw new InvalidCastException($"Key: {key}", ex);
            }
        }
    }
}