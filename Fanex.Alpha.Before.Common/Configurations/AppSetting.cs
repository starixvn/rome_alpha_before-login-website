// --------------------------------------------------------------------------
// <copyright file="AppSetting.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
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
        public static string Version
        {
            get 
            {
                return Converter.ToString2(ConfigurationManager.AppSettings["AppVersion"]); 
            }
        }

        public static string FpsActivator
        {
            get
            {
                return Converter.ToString2(ConfigurationManager.AppSettings["FPS_Activator"]);
            }
        }
    }
}
