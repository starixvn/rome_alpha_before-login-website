using Fanex.Alpha.Before.Configurations;
using Fanex.Alpha.Before.Constants;
using Fanex.Alpha.Before.Helpers;
using Microsoft.Extensions.Caching.Distributed;

namespace Fanex.Alpha.Before.Services.SignIn
{
    public interface IDistributedCacheService
    {
        Task<int> GetLoginUserFailedCount(string username);

        Task ResetLoginSession(string username);

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

        private string GetLoginUserFailedCountSessionKey(string username)
        {
            return string.Format(SessionConstants.LOGINUSERFAILEDCOUNT, AppSetting.SiteGroup, _ipHelper.ClientIP(), username);
        }

        public Task<int> GetLoginUserFailedCount(string username)
        {
            return _cacheHelper.Get(GetLoginUserFailedCountSessionKey(username), 0);
        }

        public async Task ResetLoginSession(string username)
        {
            await _cacheHelper.Remove(GetLoginUserFailedCountSessionKey(username));
        }

        public async Task<int> IncreaseLoginUserFailedCount(string username, int notInteractionTimeInMinutes)
        {
            var loginUserFailedCount = (await GetLoginUserFailedCount(username)) + 1;
            await _cacheHelper.Set(GetLoginUserFailedCountSessionKey(username), loginUserFailedCount, new DistributedCacheEntryOptions() { SlidingExpiration = TimeSpan.FromMinutes(notInteractionTimeInMinutes) });
            return loginUserFailedCount;
        }
    }
}
