// -----------------------------------------------------------------------
// <copyright file="DomainGroupSection.cs" company="Nexcel Solution">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Razorbill.Sphinx.Core.Configurations
{
    using System.Configuration;

    /// <summary>
    /// Domain group section.
    /// </summary>
    public class DomainGroupSection : ConfigurationSection
    {
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

        public static DomainGroupSection Instance
        {
            get
            {
                var domainGroupSection =
                 ConfigurationManager.GetSection("DomainGroupSection") as DomainGroupSection;
                return domainGroupSection;
            }
        }
    }

    public class DomainGroupCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        public void Add(DomainGroupElement DomainGroupElement)
        {
            BaseAdd(DomainGroupElement);
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }

        public void Remove(DomainGroupElement DomainGroupElement)
        {
            if (BaseIndexOf(DomainGroupElement) >= 0)
                BaseRemove(DomainGroupElement.Name);
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

        public int IndexOf(DomainGroupElement DomainGroupElement)
        {
            return BaseIndexOf(DomainGroupElement);
        }

        protected override sealed ConfigurationElement CreateNewElement()
        {
            return new DomainGroupElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((DomainGroupElement)element).Id;
        }
    }

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
