// -----------------------------------------------------------------------
// <copyright file="DomainGroupCollection.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Alpha.Before.Common.Configurations.Domain
{
    using System.Configuration;

    /// <summary>
    /// Domain Group Element.
    /// </summary>
    public class DomainGroupCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        protected override string ElementName
        {
            get { return "DomainGroup"; }
        }

        public new DomainGroupElement this[string id]
        {
            get
            {
                return (DomainGroupElement)BaseGet(id);
            }
        }

        public DomainGroupElement this[int index]
        {
            get { return (DomainGroupElement)BaseGet(index); }
        }

        public int IndexOf(DomainGroupElement domainGroupElement)
        {
            return BaseIndexOf(domainGroupElement);
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new DomainGroupElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((DomainGroupElement)element).Id;
        }
    }
}
