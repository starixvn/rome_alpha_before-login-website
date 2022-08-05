// -----------------------------------------------------------------------
// <copyright file="LimitedCountriesCollection.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Alpha.Before.Common.Configurations.LimtedCountryAccess
{
    using System.Configuration;

    /// <summary>
    /// Limited Countries Collection.
    /// </summary>
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
}
