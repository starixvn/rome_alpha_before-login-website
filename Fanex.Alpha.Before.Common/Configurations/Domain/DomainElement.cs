// -----------------------------------------------------------------------
// <copyright file="DomainElement.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Alpha.Before.Common.Configurations.Domain
{
    using System.Configuration;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DomainElement : ConfigurationElement
    {
        [ConfigurationProperty("active", DefaultValue = true, IsRequired = true)]
        public bool Active
        {
            get
            {
                return (bool)this["active"];
            }

            set
            {
                this["active"] = value;
            }
        }

        [ConfigurationProperty("before", DefaultValue = "NA", IsRequired = true, IsKey = true)]
        public string BeforeUrl
        {
            get
            {
                return (string)this["before"];
            }

            set
            {
                this["before"] = value;
            }
        }

        [ConfigurationProperty("after", DefaultValue = "NA", IsRequired = true)]
        public string AfterUrl
        {
            get
            {
                return (string)this["after"];
            }

            set
            {
                this["after"] = value;
            }
        }

        [ConfigurationProperty("sitekey")]
        public string SiteKey
        {
            get
            {
                return (string)this["sitekey"];
            }

            set
            {
                this["sitekey"] = value;
            }
        }

        [ConfigurationProperty("secretkey")]
        public string SecretKey
        {
            get
            {
                return (string)this["secretkey"];
            }

            set
            {
                this["secretkey"] = value;
            }
        }

        [ConfigurationProperty("proxy")]
        public string Proxy
        {
            get
            {
                return (string)this["proxy"];
            }

            set
            {
                this["proxy"] = value;
            }
        }
    }
}
