// -----------------------------------------------------------------------
// <copyright file="LimitedCountriesElement.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Alpha.Before.Common.Configurations.LimtedCountryAccess
{
    using System;
    using System.Configuration;

    /// <summary>
    /// Limited Countries Element.
    /// </summary>
    public class LimitedCountriesElement : ConfigurationElement
    {
        [ConfigurationProperty("id", DefaultValue = "NA", IsRequired = true, IsKey = true)]
        public string Id
        {
            get { return (string)this["id"]; }
            set { this["id"] = value; }
        }

        [ConfigurationProperty("name", DefaultValue = "NA", IsRequired = true, IsKey = false)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("active", DefaultValue = true, IsRequired = true, IsKey = false)]
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

        [ConfigurationProperty("affecteddate", IsRequired = true, IsKey = false)]
        public DateTime AffectedDate
        {
            get
            {
                return (DateTime)this["affecteddate"];
            }

            set
            {
                this["affecteddate"] = value;
            }
        }

        [ConfigurationProperty("description", DefaultValue = "", IsRequired = false, IsKey = false)]
        public string Description
        {
            get
            {
                var description = (string)this["description"];
                return string.IsNullOrWhiteSpace(description) ? string.Empty : description.Trim();
            }

            set
            {
                this["description"] = value;
            }
        }

        [ConfigurationProperty("istesting", DefaultValue = false, IsRequired = false, IsKey = false)]
        public bool IsTesting
        {
            get
            {
                return (bool)this["istesting"];
            }

            set
            {
                this["istesting"] = value;
            }
        }

        [ConfigurationProperty("testingip", DefaultValue = "", IsKey = false)]
        public string TestingIp
        {
            get
            {
                var iptesting = (string)this["testingip"];
                return string.IsNullOrWhiteSpace(iptesting) ? string.Empty : iptesting.Trim();
            }

            set
            {
                this["testingip"] = value;
            }
        }
    }
}
