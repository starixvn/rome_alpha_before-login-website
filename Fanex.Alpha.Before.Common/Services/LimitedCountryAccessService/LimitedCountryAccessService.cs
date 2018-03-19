namespace Fanex.Alpha.Before.Common.Configurations.LimitedCountryAccessService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Criteria.LimitedCountryAccess;
    using Models;
    using Repositories.LimitedCountryAccess;
    using Services;
    using Services.LimitedCountryAccessService;

    public class LimitedCountryAccessService : ILimitedCountryAccessService
    {
        private const string RestrictedIPRangesCachedKey = "Fanex.Alpha.Before.Common.RestrictedIPRangesCachedKey_IPV";
        private const string AllowedAccessIPsCachedKey = "Fanex.Alpha.Before.Common.AllowedAccessIPsCachedKey";

        private readonly ILimitedCountryAccessRepository ipRepository;
        private readonly IPService ipService;
        private readonly ICacheService cacheService;
        private readonly ILimitedCountriesConfigService limitedCountriesConfigService;


        public LimitedCountryAccessService(
            ILimitedCountryAccessRepository ipRepository = null,
            IPService ipService = null,
            ICacheService cacheService = null,
            ILimitedCountriesConfigService limitedCountriesConfigService = null)
        {
            this.ipRepository = ipRepository ?? new LimitedCountryAccessRepository();
            this.ipService = ipService ?? new IpServiceImpl();
            this.cacheService = cacheService ?? new CacheServiceImpl();
            this.limitedCountriesConfigService = limitedCountriesConfigService ?? new LimitedCountriesConfigService();
        }

        public bool LimitedAccess()
        {
            var activeRestrictedCountriesConfig = limitedCountriesConfigService.GetActiveRestrictedCountriesConfig();
            if (!activeRestrictedCountriesConfig.Any())
            {
                return false;
            }

            var clientIP = ipService.GetClientIp();

            if (IsTestedIP(activeRestrictedCountriesConfig, clientIP))
            {
                return true;
            }

            var isIpV6 = IpServiceImpl.IsIPV6(clientIP);
            var restrictedIPs = GetIPRangesByRestrictedCountry(activeRestrictedCountriesConfig, isIpV6);
            var ipNumber = isIpV6
                ? IpServiceImpl.ConvertIPV6ToNumber(clientIP)
                : IpServiceImpl.ConvertIPV4ToNumber(clientIP);

            var range = restrictedIPs.FirstOrDefault(ipRange => ipRange.IPFrom <= ipNumber && ipNumber <= ipRange.IPTo);
            if (range == null)
            {
                return false;
            }

            var allowedAccessIps = GetsAllowedAccessIPs();
            var result = !allowedAccessIps.Any(ip => ip == clientIP);

            return result;
        }

        public IList<IpRange> GetIPRangesByRestrictedCountry(
            IEnumerable<LimitedCountriesConfigModel> activeLimitedCountriesConfig,
            bool isIpV6)
        {
            var cacheKey = RestrictedIPRangesCachedKey + (isIpV6 ? "6" : "4");
            var restrictedIpRanges = cacheService.GetOrSet(
                cacheKey,
                () =>
                {
                    var restrictedCountries = string.Join(",", activeLimitedCountriesConfig.Select(m => m.Name));
                    var criteria = new GetIpRangesByCountryCriteria(isIpV6, restrictedCountries);
                    return ipRepository.GetIPRangesByCountry(criteria);
                },
                TimeSpan.FromDays(30));

            return restrictedIpRanges;
        }

        public IList<string> GetsAllowedAccessIPs()
        {
            var allowedAccessIPs = cacheService.GetOrSet(
                AllowedAccessIPsCachedKey,
                () => ipRepository.GetsAllowedAccessIPs(new GetAllowedAccessIPsCriteria()),
                 TimeSpan.FromMinutes(10));

            return allowedAccessIPs;
        }

        private bool IsTestedIP(IEnumerable<LimitedCountriesConfigModel> activeLimitedCountriesConfig, string clientIP)
        {
            return activeLimitedCountriesConfig.Any(m => m.IsTesting && m.TestingIp.Split(',').Contains(clientIP));
        }
    }
}