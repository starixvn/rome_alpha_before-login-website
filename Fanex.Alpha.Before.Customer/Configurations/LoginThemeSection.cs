// -----------------------------------------------------------------------
// <copyright file="LoginThemeSection.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Alpha.Before.Customer.Configurations
{
    using System;
    using System.Configuration;

    /// <summary>
    /// Login Theme Section.
    /// </summary>
    public class LoginThemeSection : ConfigurationSection
    {
        private static LoginThemeSection _instance;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">Error when parsing configuration section AlphaSetting - LoginTheme.</exception>
        public static LoginThemeSection Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = (LoginThemeSection)ConfigurationManager.GetSection("AlphaSetting/LoginTheme");
                }

                return _instance;
            }
        }

        [ConfigurationProperty("StartDateShowEventTheme", IsRequired = false)]
        public string StartDateShowEventTheme
        {
            get
            {
                return (string)base["StartDateShowEventTheme"];
            }
        }

        [ConfigurationProperty("EndDateShowEventTheme", IsRequired = false)]
        public string EndDateShowEventTheme
        {
            get
            {
                return (string)base["EndDateShowEventTheme"];
            }
        }

        [ConfigurationProperty("EventTheme", IsRequired = false, DefaultValue = "event")]
        public string EventTheme
        {
            get
            {
                return (string)base["EventTheme"];
            }
        }

        [ConfigurationProperty("DefaultTheme", IsRequired = false, DefaultValue = "default")]
        public string DefaultTheme
        {
            get
            {
                return (string)base["DefaultTheme"];
            }
        }
    }
}
