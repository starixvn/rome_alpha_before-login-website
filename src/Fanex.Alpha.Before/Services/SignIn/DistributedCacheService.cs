using Fanex.Alpha.Before.Configurations;
using Fanex.Alpha.Before.Constants;
using Fanex.Alpha.Before.Helpers;
using Fanex.Caching;
using Fanex.Caching.Redis;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace Fanex.Alpha.Before.Services.SignIn
{
    public interface IDistributedCacheService
    {
        bool IsLoginIPBlocked();

        void SetLoginIPBlocked(int blockLoginMinute);

        int GetLoginIPFailedCount();

        int GetLoginUserFailedCount(string username);

        void ResetLoginSession(string username);

        int IncreaseLoginIPFailedCount();

        int IncreaseLoginUserFailedCount(string username, int notInteractionTimeInMinutes);
    }

    public class DistributedCacheService : IDistributedCacheService
    {
        private readonly IPHelper _ipHelper;
        private readonly ICacheService _cacheService;

        public DistributedCacheService(IPHelper ipHelper, IConfiguration configuration)
        {
            _ipHelper = ipHelper;

            var options = new RedisCacheOptions();
            var configSection = configuration.GetSection("Fanex.Caching.Redis");
            configSection.Bind(options);
            ReadEndPoints(configSection, options);
            _cacheService = new CacheService(new RedisCache(options));
        }

        private string _loginIPFailedCountSessionKey => string.Format(SessionConstants.LOGINIPFAILEDCOUNT, AppSetting.SiteGroup, _ipHelper.ClientIP());
        private string _loginIPBlockedSessionKey => string.Format(SessionConstants.LOGINIPBLOCKED, AppSetting.SiteGroup, _ipHelper.ClientIP());
        private string GetLoginUserFailedCountSessionKey(string username)
        {
            return string.Format(SessionConstants.LOGINUSERFAILEDCOUNT, AppSetting.SiteGroup, _ipHelper.ClientIP(), username);
        }

        public bool IsLoginIPBlocked()
        {
            return _cacheService.Contain(_loginIPBlockedSessionKey);
        }

        public void SetLoginIPBlocked(int blockLoginMinute)
        {
            _cacheService.Remove(_loginIPFailedCountSessionKey);
            _cacheService.Set(_loginIPBlockedSessionKey, DateTime.UtcNow, new CacheItemOptions
            {
                AbsoluteExpiration = DateTime.UtcNow.AddMinutes(blockLoginMinute)
            });
        }

        public int GetLoginIPFailedCount()
        {
            return _cacheService.Get<int>(_loginIPFailedCountSessionKey);
        }

        public int GetLoginUserFailedCount(string username)
        {
            return _cacheService.Get<int>(GetLoginUserFailedCountSessionKey(username));
        }

        public void ResetLoginSession(string username)
        {
            _cacheService.Remove(_loginIPFailedCountSessionKey);
            _cacheService.Remove(_loginIPBlockedSessionKey);
            _cacheService.Remove(GetLoginUserFailedCountSessionKey(username));
        }

        public int IncreaseLoginIPFailedCount()
        {
            var loginIPFailedCount = GetLoginIPFailedCount() + 1;
            _cacheService.Set(_loginIPFailedCountSessionKey, loginIPFailedCount);
            return loginIPFailedCount;
        }

        public int IncreaseLoginUserFailedCount(string username, int notInteractionTimeInMinutes)
        {
            var loginUserFailedCount = GetLoginUserFailedCount(username) + 1;
            _cacheService.Set(GetLoginUserFailedCountSessionKey(username), loginUserFailedCount, new CacheItemOptions() { SlidingExpiration = TimeSpan.FromMinutes(notInteractionTimeInMinutes) });
            return loginUserFailedCount;
        }

        private static void ReadEndPoints(IConfigurationSection configuration, RedisCacheOptions options)
        {
            var endPoints = configuration.GetSection("ConnectionOptions:EndPoints").Get<string[]>();

            if (endPoints != null && endPoints.Length > 0)
            {
                options.ConnectionOptions ??= new ConfigurationOptions();

                foreach (var endPoint in endPoints)
                {
                    options.ConnectionOptions.EndPoints.Add(endPoint);
                }
            }
        }
    }
}
