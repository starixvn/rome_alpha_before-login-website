// -----------------------------------------------------------------------
// <copyright file="AutomationTestSettings.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Alpha.Before.Customer.Configurations
{
    using System;
    using System.Configuration;

    /// <summary>
    /// Automation Testing Section.
    /// </summary>
    public class AutomationTestSettings : ConfigurationSection
    {
        private static AutomationTestSettings _instance;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">
        /// Error when parsing configuration section AlphaSetting - AutomationTestSettings.</exception>
        public static AutomationTestSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = (AutomationTestSettings)ConfigurationManager.GetSection("AlphaSetting/AutomationTestSettings");
                }

                return _instance;
            }
        }

        [ConfigurationProperty("allowedIPs", IsRequired = false, DefaultValue = "127.0.0.1")]
        public string AllowsIPsToAccess
        {
            get
            {
                return (string)base["allowedIPs"];
            }
        }

        [ConfigurationProperty("captcha", IsRequired = false, DefaultValue = "")]
        public string Captcha
        {
            get
            {
                return (string)base["captcha"];
            }
        }

        [ConfigurationProperty("allowusernamecontainwords", IsRequired = false, DefaultValue = "")]
        public string AllowUsernameContainWords
        {
            get
            {
                return (string)base["allowusernamecontainwords"];
            }
        }
    }
}
