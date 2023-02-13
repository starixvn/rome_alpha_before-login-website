using Fanex.Caching;
using Fanex.Data.Repository;
using Fanex.Alpha.Before.Models;
using Fanex.Alpha.Before.Criteria.LimitedCountryAccess;
using Fanex.Alpha.Before.Configurations;

namespace Fanex.Alpha.Before.Services
{
    public interface ILimitedCountryAccessService
    {
        bool IsLimitedAccess(string clientIP);
    }

    public class LimitedCountryAccessService : ILimitedCountryAccessService
    {
        private const string RestrictedIPRangesCachedKey = "Fanex.Alpha.Before.Common.RestrictedIPRangesCachedKey_IPV";
        private const string AllowedAccessIPsCachedKey = "Fanex.Alpha.Before.Common.AllowedAccessIPsCachedKey";

        private readonly ICacheService cacheService;
        private readonly IConfiguration configuration;
        private readonly Logging.ILogger logger;
        private readonly IDynamicRepository dynamicRepository;

        public LimitedCountryAccessService(
            Logging.ILogger logger,
            IDynamicRepository dynamicRepository,
            ICacheService cacheService,
            IConfiguration configuration)
        {
            this.logger = logger;
            this.dynamicRepository = dynamicRepository;
            this.cacheService = cacheService;
            this.configuration = configuration;
        }

        public bool IsLimitedAccess(string clientIP)
        {
            if (IpServiceImpl.IsIPV6(clientIP))
            {
                logger.Info($"Customer Access via IPV6: {clientIP}");
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
                ?.FirstOrDefault(
                ipRange => ipRange.IPFrom <= ipNumber
                        && ipNumber <= ipRange.IPTo);
        }

        public IEnumerable<IpRange> GetIPRangesByRestrictedCountry(
            IEnumerable<LimitedCountriesConfigModel> activeLimitedCountriesConfig,
            bool isIpV6)
        {
            var restrictedCountries = string.Join(",", activeLimitedCountriesConfig.Select(m => m.Name));
            var cacheKey = $"{RestrictedIPRangesCachedKey}{(isIpV6 ? "6" : "4")}_{restrictedCountries}";

            IEnumerable<IpRange> getDataFromDatabaseFunc()
            {
                var criteria = new GetIpRangesByCountryCriteria(isIpV6, restrictedCountries);
                return dynamicRepository.Fetch<IpRange>(criteria);
            }

            return cacheService.GetOrSet(
                cacheKey,
                () => AppSetting.TrackRunTime
                    ? FunctionRecorder.RunFunctionAndTrackTime(getDataFromDatabaseFunc, cacheKey)
                    : getDataFromDatabaseFunc(),
                new CacheItemOptions
                {
                    RelativeExpiration = TimeSpan.FromDays(30)
                });
        }

        public IEnumerable<string> GetsAllowedAccessIPs()
            => cacheService.GetOrSet(
                AllowedAccessIPsCachedKey,
                () => dynamicRepository.Fetch<string>(new GetAllowedAccessIPsCriteria()),
                new CacheItemOptions
                {
                    RelativeExpiration = TimeSpan.FromMinutes(10)
                });

        private static bool IsTestedIP(IEnumerable<LimitedCountriesConfigModel> activeLimitedCountriesConfig, string clientIP)
            => activeLimitedCountriesConfig.Any(m
                => m.IsTesting
                && m.TestingIp.Split(',').Contains(clientIP));

        private IEnumerable<LimitedCountriesConfigModel> GetActiveRestrictedCountriesConfig()
        {
            return configuration.GetSection("LimitedCountries").Get<LimitedCountry[]>()
                .Where(m => m.Active && DateTime.Now >= m.AffectedDate)
                .Select(m => new LimitedCountriesConfigModel(m.Name, m.IsTesting, m.TestingIp));
        }
    }
}