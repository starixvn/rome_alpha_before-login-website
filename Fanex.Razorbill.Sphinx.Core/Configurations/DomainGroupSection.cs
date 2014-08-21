// -----------------------------------------------------------------------
// <copyright file="DomainGroupSection.cs" company="Nexcel Solution">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Razorbill.Sphinx.Core.Configurations
{
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Domain group section.
    /// </summary>
    public class DomainGroupSection : ConfigurationSection
    {
        public static DomainGroupSection Instance
        {
            get
            {
                var domainGroupSection =
                 ConfigurationManager.GetSection("DomainGroupSection") as DomainGroupSection;
                return domainGroupSection;
            }
        }

        [ConfigurationProperty("DomainGroup", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(DomainGroupCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public DomainGroupCollection DomainGroup
        {
            get
            {
                var domainGroupCollection =
                    (DomainGroupCollection)base["DomainGroup"];
                return domainGroupCollection;
            }
        }
    }

    /// <summary>
    /// Domain Group Collection.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", 
        "SA1402:FileMayOnlyContainASingleClass", 
        Justification = "its ok here")]
    public class DomainGroupCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }
        
        public DomainGroupElement this[int index]
        {
            get
            {
                return (DomainGroupElement)BaseGet(index);
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

        public new DomainGroupElement this[string name]
        {
            get
            {
                return (DomainGroupElement)BaseGet(name);
            }
        }

        public void Add(DomainGroupElement domainGroupElement)
        {
            BaseAdd(domainGroupElement);
        }
        
        /// <summary>
        /// Remove element.
        /// </summary>
        /// <param name="domainGroupElement">Domain Group Element.</param>
        public void Remove(DomainGroupElement domainGroupElement)
        {
            if (BaseIndexOf(domainGroupElement) >= 0)
            {
                BaseRemove(domainGroupElement.Name);
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
        /// <param name="domainGroupElement">Domain Group Element.</param>
        /// <returns>Return index.</returns>
        public int IndexOf(DomainGroupElement domainGroupElement)
        {
            return BaseIndexOf(domainGroupElement);
        }

        protected override sealed ConfigurationElement CreateNewElement()
        {
            return new DomainGroupElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((DomainGroupElement)element).Id;
        }
        
        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }
    }

    /// <summary>
    /// Domain Group Element.
    /// </summary>
    public class DomainGroupElement : ConfigurationElement
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

        [ConfigurationProperty("domains", DefaultValue = "", IsRequired = true, IsKey = false)]
        public string Domains
        {
            get
            {
                var domains = (string)this["domains"];
                return string.IsNullOrWhiteSpace(domains) ? string.Empty : domains.Trim();
            }

            set
            {
                this["domains"] = value;
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
    }
}
