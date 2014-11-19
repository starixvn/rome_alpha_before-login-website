// -----------------------------------------------------------------------
// <copyright file="LimitedCountryAccessService.cs" company="Nexcel Solution">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Razorbill.Sphinx.Service.Common.LimitedCountryAccess
{
    using System;
    using System.Collections;
    using System.Linq;
    using Fanex.Common;
    using Fanex.Data;
    using Fanex.Razorbill.Sphinx.Core.Configurations;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LimitedCountryAccessService
    {
        public LimitedCountryAccessService()
        {
        }

        /// <summary>
        /// Checking Limited country access.
        /// </summary>
        /// <returns>Return blocking status.</returns>
        public bool LimitedAccess()
        {
            var limitedCountryAccessSection = LimtedCountryAccessSection.Instance.LimitedCountries;
            if (limitedCountryAccessSection == null || 
                limitedCountryAccessSection.Count == 0)
            {
                return false;
            }

            var limitedCountriesActive = from m in limitedCountryAccessSection.Cast<LimitedCountriesElement>().Where(m => m.Active) select m;

            if (!limitedCountriesActive.Any())
            {
                return false;
            }

            var customerCountryCode = from m in limitedCountriesActive 
                                      where m.Name.ToUpper().Equals(CountryCodeByIp().ToUpper()) && m.AffectedDate >= DateTime.Now 
                                      select m;
            if (customerCountryCode.Any())
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Get country code by customer's IP.
        /// </summary>
        /// <returns>Return country code.</returns>
        private string CountryCodeByIp()
        {
            using (ObjectDb objectDb = new ObjectDb("Age_CheckValidCountry"))
            {
                objectDb.AssignParamValues(new Hashtable()
                {
                    { "ip", Fanex.Razorbill.Sphinx.Core.Extensions.IPExtension.CustomerIp() },
                    { "countrycode", string.Empty }
                });

                objectDb.ExecuteNonQuery();

                return Converter.ToString2(objectDb.GetOuputParams()["countrycode"]);
            }
        }
    }
}
