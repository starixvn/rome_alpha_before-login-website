// -----------------------------------------------------------------------
// <copyright file="AutomationTestingSection.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Alpha.Before.Customer.Configurations
{
    using System;
    using System.Configuration;

    /// <summary>
    /// Automation Testing Section.
    /// </summary>
    public class AutomationTestingSection : ConfigurationSection
    {
        private static AutomationTestingSection _instance;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">Error when parsing configuration section AlphaSetting - AutomationTestingSection.</exception>
        public static AutomationTestingSection Instance
        {
            get
            {
                if (_instance == null)
                {
                    try
                    {
                        _instance = (AutomationTestingSection)ConfigurationManager.GetSection("AlphaSetting/AutomationTestingSection");
                    }
                    catch (Exception e)
                    {
                        throw new ConfigurationErrorsException("Error when parsing configuration section AlphaSetting - AutomationTestingSection", e);
                    }
                }

                return _instance;
            }
        }

        [ConfigurationProperty("allowedIPs", IsRequired = true, DefaultValue = "")]
        public string AllowedIPs
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

        [ConfigurationProperty("usernameContainSpecialCharacters", IsRequired = false, DefaultValue = "test")]
        public string UsernameContainSpecialCharacters
        {
            get
            {
                return (string)base["usernameContainSpecialCharacters"];
            }
        }
    }
}
