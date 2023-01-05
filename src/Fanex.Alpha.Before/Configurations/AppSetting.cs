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

        public static T GetValue<T>(string key)
        {
            try
            {
                return configurationRoot.GetSection(key).Get<T>();
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