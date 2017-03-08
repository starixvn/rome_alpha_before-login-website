// --------------------------------------------------------------------------
// <copyright file="AppSetting.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.Alpha.Before.Common.Configurations
{
    using System.Configuration;
    using Fanex.Common;

    /// <summary>
    /// Application Setting.
    /// </summary>
    public static class AppSetting
    {
        private static string version;
        private static string fpsActivator;
        private static string googleAnalyticId;

        static AppSetting()
        {
            version = Converter.ToString2(ConfigurationManager.AppSettings["AppVersion"]);
            fpsActivator = Converter.ToString2(ConfigurationManager.AppSettings["FPS_Activator"]);
            googleAnalyticId = Converter.ToString2(ConfigurationManager.AppSettings["GoogleAnalyticId"]);
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
    }
}