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
        }

        public static string Version
        {
            get
            {
                return version;
            }
        }

        public static string FpsActivator
        {
            get
            {
                return fpsActivator;
            }
        }

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