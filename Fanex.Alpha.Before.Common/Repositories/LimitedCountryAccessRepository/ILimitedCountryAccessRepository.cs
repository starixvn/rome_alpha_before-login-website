namespace Fanex.Alpha.Before.Common.Repositories.LimitedCountryAccessRepository
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
