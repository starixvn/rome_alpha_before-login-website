using Fanex.Alpha.Before.Configurations;
using Fanex.Alpha.Before.Constants;
using Fanex.Alpha.Before.Helpers;
using Microsoft.Extensions.Caching.Distributed;

namespace Fanex.Alpha.Before.Services.SignIn
{
    public interface IDistributedCacheService
    {
        Task<bool> IsLoginIPBlocked();

        Task SetLoginIPBlocked(int blockLoginMinute);

        Task<int> GetLoginIPFailedCount();

        Task<int> GetLoginUserFailedCount(string username);

        Task ResetLoginSession(string username);

        Task<int> IncreaseLoginIPFailedCount();

        Task<int> IncreaseLoginUserFailedCount(string username, int notInteractionTimeInMinutes);
    }

    public class DistributedCacheService : IDistributedCacheService
    {
        private readonly IPHelper _ipHelper;
        private readonly IDistributedCacheHelper _cacheHelper;

        public DistributedCacheService(IPHelper ipHelper, IDistributedCacheHelper cacheHelper)
        {
            _ipHelper = ipHelper;
            _cacheHelper = cacheHelper;
        }

        private string _loginIPFailedCountSessionKey => string.Format(SessionConstants.LOGINIPFAILEDCOUNT, AppSetting.SiteGroup, _ipHelper.ClientIP());
        private string _loginIPBlockedSessionKey => string.Format(SessionConstants.LOGINIPBLOCKED, AppSetting.SiteGroup, _ipHelper.ClientIP());
        private string GetLoginUserFailedCountSessionKey(string username)
        {
            return string.Format(SessionConstants.LOGINUSERFAILEDCOUNT, AppSetting.SiteGroup, _ipHelper.ClientIP(), username);
        }

        public Task<bool> IsLoginIPBlocked()
        {
            return _cacheHelper.Contain(_loginIPBlockedSessionKey);
        }

        public async Task SetLoginIPBlocked(int blockLoginMinute)
        {
            await _cacheHelper.Remove(_loginIPFailedCountSessionKey);
            await _cacheHelper.Set(_loginIPBlockedSessionKey, DateTime.UtcNow, new DistributedCacheEntryOptions()
            {
                AbsoluteExpiration = DateTime.UtcNow.AddMinutes(blockLoginMinute)
            });
        }

        public Task<int> GetLoginIPFailedCount()
        {
            return _cacheHelper.Get(_loginIPFailedCountSessionKey, 0);
        }

        public Task<int> GetLoginUserFailedCount(string username)
        {
            return _cacheHelper.Get(GetLoginUserFailedCountSessionKey(username), 0);
        }

        public async Task ResetLoginSession(string username)
        {
            await _cacheHelper.Remove(_loginIPFailedCountSessionKey);
            await _cacheHelper.Remove(_loginIPBlockedSessionKey);
            await _cacheHelper.Remove(GetLoginUserFailedCountSessionKey(username));
        }

        public async Task<int> IncreaseLoginIPFailedCount()
        {
            var loginIPFailedCount = (await GetLoginIPFailedCount()) + 1;
            await _cacheHelper.Set(_loginIPFailedCountSessionKey, loginIPFailedCount);
            return loginIPFailedCount;
        }

        public async Task<int> IncreaseLoginUserFailedCount(string username, int notInteractionTimeInMinutes)
        {
            var loginUserFailedCount = (await GetLoginUserFailedCount(username)) + 1;
            await _cacheHelper.Set(GetLoginUserFailedCountSessionKey(username), loginUserFailedCount, new DistributedCacheEntryOptions() { SlidingExpiration = TimeSpan.FromMinutes(notInteractionTimeInMinutes) });
            return loginUserFailedCount;
        }
    }
}
