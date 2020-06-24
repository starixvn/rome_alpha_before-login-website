namespace Fanex.Alpha.Before.Common.Configurations
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
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
            IsEnableFPS = !string.IsNullOrEmpty(FpsActivator) && !string.IsNullOrEmpty(ConfigurationManager.AppSettings["FPS_API"]);

            RobotExcludeSuperIds = ToIntDictionary(ConfigurationManager.AppSettings["Robot:ExcludeSuperIds"]);
            RobotExcludeSites = ToDictionary(ConfigurationManager.AppSettings["Robot:ExcludeSites"]);
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

        public static bool IsEnableFPS { get; }

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

        public static IDictionary<int, bool> RobotExcludeSuperIds { get; }

        public static IDictionary<string, bool> RobotExcludeSites { get; }

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

        public static bool IsIgnoreRobotCaptcha(int superId, string site)
            => RobotExcludeSuperIds.ContainsKey(superId)
            || RobotExcludeSites.ContainsKey(site.ToUpperInvariant());
    }
}