namespace Fanex.Alpha.Before.Common.Services.LimitedCountryAccessService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configurations.LimtedCountryAccess;
    using Models;

    public class LimitedCountriesConfigService : ILimitedCountriesConfigService
    {
        public IEnumerable<LimitedCountriesConfigModel> GetActiveRestrictedCountriesConfig()
        {
            var limitedCountryAccessSection = LimtedCountryAccessSection.Instance.LimitedCountries;
            if (limitedCountryAccessSection == null || limitedCountryAccessSection.Count == 0)
            {
                return Enumerable.Empty<LimitedCountriesConfigModel>();
            }

            return limitedCountryAccessSection.Cast<LimitedCountriesElement>()
                .Where(m => m.Active && DateTime.Now >= m.AffectedDate)
                .Select(m => new LimitedCountriesConfigModel(m.Name, m.IsTesting, m.TestingIp));
        }
    }
}
