// -----------------------------------------------------------------------
// <copyright file="LimtedCountryAccessSection.cs" company="Nexcel Solution">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Razorbill.Sphinx.Core.Configurations
{
    using System;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;

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

    /// <summary>
    /// Limited Countries Collection.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", 
        "SA1402:FileMayOnlyContainASingleClass", 
        Justification = "its ok here")]
    public class LimitedCountriesCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }
        
        public LimitedCountriesElement this[int index]
        {
            get
            {
                return (LimitedCountriesElement)BaseGet(index);
            }

            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }

                BaseAdd(index, value);
            }
        }

        public new LimitedCountriesElement this[string name]
        {
            get
            {
                return (LimitedCountriesElement)BaseGet(name);
            }
        }

        public void Add(LimitedCountriesElement limitedCountriesElement)
        {
            BaseAdd(limitedCountriesElement);
        }
        
        /// <summary>
        /// Remove element.
        /// </summary>
        /// <param name="limitedCountriesElement">Domain Group Element.</param>
        public void Remove(LimitedCountriesElement limitedCountriesElement)
        {
            if (BaseIndexOf(limitedCountriesElement) >= 0)
            {
                BaseRemove(limitedCountriesElement.Name);
            }
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string alias)
        {
            BaseRemove(alias);
        }

        public void Clear()
        {
            BaseClear();
        }

        /// <summary>
        /// Get Index of.
        /// </summary>
        /// <param name="limitedCountriesElement">Limited Countries Element.</param>
        /// <returns>Return index.</returns>
        public int IndexOf(LimitedCountriesElement limitedCountriesElement)
        {
            return BaseIndexOf(limitedCountriesElement);
        }

        protected override sealed ConfigurationElement CreateNewElement()
        {
            return new LimitedCountriesElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((LimitedCountriesElement)element).Id;
        }
        
        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }
    }

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
