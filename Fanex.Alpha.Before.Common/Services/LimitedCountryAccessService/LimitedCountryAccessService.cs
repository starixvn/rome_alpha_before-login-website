namespace Fanex.Alpha.Before.Common.Configurations.LimitedCountryAccessService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Criteria.LimitedCountryAccess;
    using LimtedCountryAccess;
    using Models;
    using Repositories.LimitedCountryAccessRepository;
    using Services;

    public class LimitedCountryAccessService : ILimitedCountryAccessService
    {
        private const string RestrictedIPRangesCachedKey = "Fanex.Alpha.Before.Common.RestrictedIPRangesCachedKey_IPV";
        private const string AllowedAccessIPsCachedKey = "Fanex.Alpha.Before.Common.AllowedAccessIPsCachedKey";

        private readonly ILimitedCountryAccessRepository ipRepository;
        private readonly IPService ipService;
        private readonly ICacheService cacheService;

        public LimitedCountryAccessService(
            ILimitedCountryAccessRepository ipRepository = null,
            IPService ipService = null,
            ICacheService cacheService = null)
        {
            this.ipRepository = ipRepository ?? new LimitedCountryAccessRepository();
            this.ipService = ipService ?? new IpServiceImpl();
            this.cacheService = cacheService ?? new CacheServiceImpl();
        }

        public bool LimitedAccess()
        {
            var activeRestrictedCountriesConfig = GetActiveRestrictedCountriesConfig();
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

        public IEnumerable<LimitedCountriesElement> GetActiveRestrictedCountriesConfig()
        {
            var limitedCountryAccessSection = LimtedCountryAccessSection.Instance.LimitedCountries;
            if (limitedCountryAccessSection == null || limitedCountryAccessSection.Count == 0)
            {
                return new List<LimitedCountriesElement>();
            }

            return limitedCountryAccessSection.Cast<LimitedCountriesElement>().Where(m => m.Active && DateTime.Now >= m.AffectedDate);
        }

        public IList<IpRange> GetIPRangesByRestrictedCountry(
            IEnumerable<LimitedCountriesElement> activeLimitedCountriesConfig,
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

        private bool IsTestedIP(IEnumerable<LimitedCountriesElement> activeLimitedCountriesConfig, string clientIP)
        {
            return activeLimitedCountriesConfig.Any(m => m.IsTesting && m.TestingIp.Split(',').Contains(clientIP));
        }
    }
}