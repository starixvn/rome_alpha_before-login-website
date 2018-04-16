// -----------------------------------------------------------------------
// <copyright file="AlphaCommonSection.cs" company="">
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
    public class AlphaCommonSection : ConfigurationSection
    {
        private static AlphaCommonSection _instance;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">Error when parsing configuration section AlphaSetting - CommonSettings.</exception>
        public static AlphaCommonSection Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = (AlphaCommonSection)ConfigurationManager.GetSection("AlphaSetting/CommonSettings");
                }

                return _instance;
            }
        }

        [ConfigurationProperty("OfficeIPs", IsRequired = false)]
        public string OfficeIPs
        {
            get
            {
                return (string)base["OfficeIPs"];
            }
        }

        [ConfigurationProperty("LoginFailTimeToStartUsingCaptcha", IsRequired = false)]
        public int LoginFailTimeToStartUsingCaptcha
        {
            get
            {
                return (int)base["LoginFailTimeToStartUsingCaptcha"];
            }
        }

        [ConfigurationProperty("PasswordMaxFailTimeForOneUser", IsRequired = false)]
        public int PasswordMaxFailTimeForOneUser
        {
            get
            {
                return (int)base["PasswordMaxFailTimeForOneUser"];
            }
        }

        [ConfigurationProperty("NoInteractionTimeInMinutes", IsRequired = false)]
        public int NoInteractionTimeInMinutes
        {
            get
            {
                return (int)base["NoInteractionTimeInMinutes"];
            }
        }

        [ConfigurationProperty("MaxTimesToAllowLoginFail", IsRequired = false)]
        public int MaxTimesToAllowLoginFail
        {
            get
            {
                return (int)base["MaxTimesToAllowLoginFail"];
            }
        }

        [ConfigurationProperty("BlockLoginMinutes", IsRequired = false)]
        public int BlockLoginMinutes
        {
            get
            {
                return (int)base["BlockLoginMinutes"];
            }
        }
    }
}
