namespace Fanex.Alpha.Before.Common.Configurations
{
    using System.Configuration;
    using Fanex.Common;

    public static class AppSetting
    {
        private static string version;
        private static string fpsActivator;
        private static string googleAnalyticId;
        private static bool isMBCSite;
        private static readonly bool trackRunTime;

        static AppSetting()
        {
            version = Converter.ToString2(ConfigurationManager.AppSettings["AppVersion"]);
            fpsActivator = Converter.ToString2(ConfigurationManager.AppSettings["FPS_Activator"]);
            googleAnalyticId = Converter.ToString2(ConfigurationManager.AppSettings["GoogleAnalyticId"]);
            isMBCSite = !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["IsMBCSite"])
                && ConfigurationManager.AppSettings["IsMBCSite"] == "1";

            trackRunTime = ConfigurationManager.AppSettings["TrackRunTime"] == "1";
            GoogleV3KeyForFrontEnd = ConfigurationManager.AppSettings["GoogleV3KeyForFrontEnd"];
            GoogleV3KeyForBackEnd = ConfigurationManager.AppSettings["GoogleV3KeyForBackEnd"];
            IsEnableGoogleV3 = !string.IsNullOrEmpty(GoogleV3KeyForFrontEnd) && !string.IsNullOrEmpty(GoogleV3KeyForBackEnd);
            IsEnableFPS = !string.IsNullOrEmpty(FpsActivator) && !string.IsNullOrEmpty(ConfigurationManager.AppSettings["FPS_API"]);
            ProxyServer = ConfigurationManager.AppSettings["ProxyServer"];
        }

        public static string Version
        {
            get
            {
                return version;
            }
        }

        public static string ProxyServer { get; }

        public static string FpsActivator
        {
            get
            {
                return fpsActivator;
            }
        }

        public static bool IsEnableGoogleV3 { get; }

        public static bool IsEnableFPS { get; }

        public static string GoogleV3KeyForFrontEnd { get; }

        public static string GoogleV3KeyForBackEnd { get; }

        public static string GoogleAnalyticId
        {
            get
            {
                return googleAnalyticId;
            }
        }

        public static bool IsMBCSite
        {
            get
            {
                return isMBCSite;
            }
        }

        public static bool DisabledVietnamese => IsMBCSite;

        public static bool TrackRunTime => trackRunTime;
    }
}