// -----------------------------------------------------------------------
// <copyright file="DomainCollection.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Alpha.Before.Common.Configurations.Domain
{
    using System.Configuration;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DomainCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }
        
        public DomainElement this[int index]
        {
            get
            {
                return (DomainElement)BaseGet(index);
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

        public new DomainElement this[string id]
        {
            get
            {
                return (DomainElement)BaseGet(id);
            }
        }

        public void Add(DomainElement toteElement)
        {
            BaseAdd(toteElement);
        }

        public void Remove(DomainElement toteElement)
        {
            if (BaseIndexOf(toteElement) >= 0)
            {
                BaseRemove(toteElement.Id);
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

        public int IndexOf(DomainElement storeProcedureElement)
        {
            return BaseIndexOf(storeProcedureElement);
        }

        protected override sealed ConfigurationElement CreateNewElement()
        {
            return new DomainElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((DomainElement)element).Id;
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }
    }
}
