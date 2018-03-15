namespace Fanex.Alpha.Before.Common.Configurations.LimitedCountryAccessService
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Criteria.LimitedCountryAccess;
    using LimtedCountryAccess;
    using Logging.Extensions.Web;
    using Models;
    using Repositories.LimitedCountryAccessRepository;
    using Services;

    public class LimitedCountryAccessService : ILimitedCountryAccessService
    {
        private const string RestrictedIPRangesCachedKey = "Fanex.Alpha.Before.Common.RestrictedIPRangesCachedKey";
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
            var clientIP = ipService.GetClientIp();

            if (IpServiceImpl.IsIPV6(clientIP))
            {
                ExceptionHandler.Handle(new Exception("Customer Access via IPV6"), TraceEventType.Information);
                return false;
            }

            var activeRestrictedCountriesConfig = GetActiveRestrictedCountriesConfig();
            if (!activeRestrictedCountriesConfig.Any())
            {
                return false;
            }

            if (IsTestedIP(activeRestrictedCountriesConfig, clientIP))
            {
                return true;
            }

            var restrictedIPs = GetIPRangesByRestrictedCountry(activeRestrictedCountriesConfig);
            var ipNumber = IpServiceImpl.ConvertIPToNumber(clientIP);
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

        public IList<IpRange> GetIPRangesByRestrictedCountry(IEnumerable<LimitedCountriesElement> activeLimitedCountriesConfig)
        {
            var restrictedIpRanges = cacheService.GetOrSet(
                RestrictedIPRangesCachedKey,
                () =>
                {
                    var restrictedCountries = string.Join(",", activeLimitedCountriesConfig.Select(m => m.Name));
                    var criteria = new GetIpRangesByCountryCriteria(restrictedCountries);
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