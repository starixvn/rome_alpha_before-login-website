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
        [ConfigurationProperty("id", DefaultValue = "0", IsRequired = true, IsKey = true)]
        public string Id
        {
            get
            {
                return (string)this["id"];
            }

            set
            {
                this["id"] = value;
            }
        }

        [ConfigurationProperty("active", DefaultValue = true, IsRequired = true, IsKey = true)]
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

        [ConfigurationProperty("before", DefaultValue = "NA", IsRequired = true)]
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
    }
}
