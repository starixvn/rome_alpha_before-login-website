// -----------------------------------------------------------------------
// <copyright file="DomainGroupSection.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Alpha.Before.Common.Configurations.Domain
{
    using System.Configuration;

    /// <summary>
    /// Domain Group Section.
    /// </summary>
    public class DomainGroupSection : ConfigurationSection
    {
        public static DomainGroupSection Instance
        {
            get
            {
                return ConfigurationManager.GetSection("DomainGroupSection") as DomainGroupSection;
            }
        }

        [ConfigurationProperty("", IsDefaultCollection = true, IsRequired = true)]
        public DomainGroupCollection DomainGroup
        {
            get
            {
                return (DomainGroupCollection)base[string.Empty];
            }
        }
    }
}
