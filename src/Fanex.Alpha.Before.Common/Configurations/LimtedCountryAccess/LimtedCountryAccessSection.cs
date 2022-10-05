// -----------------------------------------------------------------------
// <copyright file="LimtedCountryAccessSection.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Alpha.Before.Common.Configurations.LimtedCountryAccess
{
    using System.Configuration;

    /// <summary>
    /// Limited Country Access section.
    /// </summary>
    public class LimtedCountryAccessSection : ConfigurationSection
    {
        public static LimtedCountryAccessSection Instance
        {
            get
            {
                return ConfigurationManager.GetSection("LimtedCountryAccessSection") as LimtedCountryAccessSection;
            }
        }

        [ConfigurationProperty("LimitedCountries", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(LimitedCountriesCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public LimitedCountriesCollection LimitedCountries
        {
            get
            {
                return (LimitedCountriesCollection)base["LimitedCountries"];
            }
        }
    }
}
