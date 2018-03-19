namespace Fanex.Alpha.Before.Common.Services.LimitedCountryAccessService
{
    using System.Collections.Generic;
    using Models;

    public interface ILimitedCountriesConfigService
    {
        IEnumerable<LimitedCountriesConfigModel> GetActiveRestrictedCountriesConfig();
    }
}
