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
        private readonly bool trackRunTime;

        public LimitedCountryAccessService(
            bool trackRunTime,
            ILimitedCountryAccessRepository ipRepository = null,
            IPService ipService = null,
            ICacheService cacheService = null,
            ILimitedCountriesConfigService limitedCountriesConfigService = null)
        {
            this.ipRepository = ipRepository ?? new LimitedCountryAccessRepository();
            this.ipService = ipService ?? new IpServiceImpl();
            this.cacheService = cacheService ?? new CacheServiceImpl();
            this.limitedCountriesConfigService = limitedCountriesConfigService ?? new LimitedCountriesConfigService();
            this.trackRunTime = trackRunTime;
        }

        public bool IsLimitedAccess()
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

            var range = GetRestrictedIPRange(activeRestrictedCountriesConfig, clientIP);
            if (range == null)
            {
                return false;
            }

            var allowedAccessIps = GetsAllowedAccessIPs();

            return !allowedAccessIps.Any(ip => ip == clientIP);
        }

        private IpRange GetRestrictedIPRange(
            IEnumerable<LimitedCountriesConfigModel> activeRestrictedCountriesConfig, 
            string clientIP)
        {
            var isIpV6 = IpServiceImpl.IsIPV6(clientIP);
            var restrictedIPRangeList = GetIPRangesByRestrictedCountry(activeRestrictedCountriesConfig, isIpV6);
            var ipNumber = isIpV6
                ? IpServiceImpl.ConvertIPV6ToNumber(clientIP)
                : IpServiceImpl.ConvertIPV4ToNumber(clientIP);

            return restrictedIPRangeList
                .FirstOrDefault(
                ipRange => ipRange.IPFrom <= ipNumber
                        && ipNumber <= ipRange.IPTo);
        }

        public IList<IpRange> GetIPRangesByRestrictedCountry(
            IEnumerable<LimitedCountriesConfigModel> activeLimitedCountriesConfig,
            bool isIpV6)
        {
            var restrictedCountries = string.Join(",", activeLimitedCountriesConfig.Select(m => m.Name));
            var cacheKey = $"{RestrictedIPRangesCachedKey}{(isIpV6 ? "6" : "4")}_{restrictedCountries}";

            Func<IList<IpRange>> getDataFromDatabaseFunc = () =>
            {
                var criteria = new GetIpRangesByCountryCriteria(isIpV6, restrictedCountries);
                return ipRepository.GetIPRangesByCountry(criteria);
            };

            return cacheService.GetOrSet(
                cacheKey,
                () => trackRunTime
                    ? FunctionRecorder.RunFunctionAndTrackTime(getDataFromDatabaseFunc, "GetIPRangesByRestrictedCountry")
                    : getDataFromDatabaseFunc(),
                TimeSpan.FromDays(30));
        }

        public IList<string> GetsAllowedAccessIPs()
            => cacheService.GetOrSet(
                AllowedAccessIPsCachedKey,
                () => ipRepository.GetsAllowedAccessIPs(new GetAllowedAccessIPsCriteria()),
                TimeSpan.FromMinutes(10));

        private bool IsTestedIP(IEnumerable<LimitedCountriesConfigModel> activeLimitedCountriesConfig, string clientIP)
            => activeLimitedCountriesConfig.Any(m
                => m.IsTesting
                && m.TestingIp.Split(',').Contains(clientIP));
    }
}