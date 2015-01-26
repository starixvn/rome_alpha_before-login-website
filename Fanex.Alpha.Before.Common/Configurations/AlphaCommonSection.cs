// -----------------------------------------------------------------------
// <copyright file="AlphaCommonSection.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
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
                    try
                    {
                        _instance = (AlphaCommonSection)ConfigurationManager.GetSection("AlphaSetting/CommonSettings");
                    }
                    catch (Exception e)
                    {
                        throw new ConfigurationErrorsException("Error when parsing configuration section AlphaSetting - LoginTheme", e);
                    }
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
    }
}
