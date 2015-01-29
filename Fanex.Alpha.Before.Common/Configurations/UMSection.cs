// -----------------------------------------------------------------------
// <copyright file="UMSection.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Alpha.Before.Common.Configurations
{
    using System;
    using System.Configuration;

    /// <summary>
    /// Under maintenance Section.
    /// </summary>
    public class UMSection : ConfigurationSection
    {
        private static UMSection _instance;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">Error when parsing configuration section AlphaSetting - UM.</exception>
        public static UMSection Instance
        {
            get
            {
                if (_instance == null)
                {
                    try
                    {
                        _instance = (UMSection)ConfigurationManager.GetSection("AlphaSetting/UM");
                    }
                    catch (Exception e)
                    {
                        throw new ConfigurationErrorsException("Error when parsing configuration section AlphaSetting - UM", e);
                    }
                }

                return _instance;
            }
        }

        [ConfigurationProperty("UMAddress", IsRequired = false)]
        public string UMAddress
        {
            get
            {
                return (string)base["UMAddress"];
            }
        }

        [ConfigurationProperty("IsUMTesting", IsRequired = false)]
        public bool IsUMTesting
        {
            get
            {
                return (bool)base["IsUMTesting"];
            }
        }

        [ConfigurationProperty("EnableCheckUM", IsRequired = false)]
        public bool EnableCheckUM
        {
            get
            {
                return (bool)base["EnableCheckUM"];
            }
        }

        [ConfigurationProperty("UMStartTesting", IsRequired = false)]
        public string UMStartTesting
        {
            get
            {
                return (string)base["UMStartTesting"];
            }
        }

        [ConfigurationProperty("UMEndTesting", IsRequired = false)]
        public string UMEndTesting
        {
            get
            {
                return (string)base["UMEndTesting"];
            }
        }
    }
}
