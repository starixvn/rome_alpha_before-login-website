using Fanex.Alpha.Before.Constants;

namespace Fanex.Alpha.Before.Configurations
{
    public static class AppSetting
    {
        private readonly static IConfigurationRoot configurationRoot = InitConfiguration();

        private static IConfigurationRoot InitConfiguration()
        {
            const string configFileName = "appsettings.json";
            const string domainMappingFileName = "domainmappings.json";

            if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), configFileName)))
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile(configFileName, optional: false, reloadOnChange: true)
                    .AddJsonFile(domainMappingFileName, optional: false, reloadOnChange: true);

                return builder.Build();
            }

            return null;
        }

        public static string SiteGroup => GetValue<string>("SiteGroup");

        public static bool Is789ySite => SiteGroup == SiteGroupConstants.Site789y;

        public static bool DisabledVietnamese => !Is789ySite;

        public static IDictionary<string, string> DomainMappings
            => configurationRoot.GetSection("DomainMappings").Get<DomainMapping[]>().ToDictionary(kvp => kvp.Before, kvp => kvp.After);

        public static string FpsActivator => GetValue<string>("FPS:Activator");

        public static string FpsSubscriber => GetValue<string>("FPS:Subscriber");

        public static string FpsApi => GetValue<string>("FPS:API");

        public static bool IsEnableFPS => !string.IsNullOrEmpty(FpsActivator) && !string.IsNullOrEmpty(FpsApi);

        public static bool TrackRunTime => GetValue<bool>("TrackRunTime");

        public static string TechnicalErrorTime => GetValue<string>("TechnicalErrorTime");

        public static bool IsShowTechnicalErrorMessage => !string.IsNullOrWhiteSpace(TechnicalErrorTime);

        public static string AssetAppPaths => GetValue<string>("AssetAppPaths");

        public static string DCSStaticAppPaths => GetValue<string>("DCSStaticAppPaths");

        public static string AfterLoginAuthorizationPage => GetValue<string>("AfterLoginPage");

        public static bool EnabledCallHashPasswordAPI => GetValue<bool>("PasswordService:IsEnabled");

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