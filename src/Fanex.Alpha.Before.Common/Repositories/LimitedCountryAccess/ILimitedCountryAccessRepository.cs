namespace Fanex.Alpha.Before.Common.Repositories.LimitedCountryAccess
{
    using System.Collections.Generic;
    using Criteria.LimitedCountryAccess;
    using Models;

    public interface ILimitedCountryAccessRepository
    {
        IList<IpRange> GetIPRangesByCountry(GetIpRangesByCountryCriteria criteria);

        IList<string> GetsAllowedAccessIPs(GetAllowedAccessIPsCriteria criteria);
    }
}
