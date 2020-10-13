namespace Fanex.Alpha.Before.Customer.Services.SignIn
{
    using Fanex.Alpha.Before.Common.Constants;
    using Fanex.Alpha.Before.Common.Helpers;
    using Fanex.Alpha.Before.Infrastructure.Extensions;
    using System;

    public interface ICacheService
    {
        string GetLastLoginUsername();

        void SetLastLoginUsername(string username);

        int GetTotallyLoginFailed();

        int GetCurrentLoginFailed();

        DateTime GetLoginFailedTime();

        void SetLoginFailedTime();

        void ResetLoginFailed();

        void ResetCurrentLoginFailed();

        int IncreaseLoginFailedCount();
    }

    public class CacheService: ICacheService
    {
        private readonly string _lastLoginUserNameSessionKey = string.Format(SessionConstants.LASTLOGINUSERNAME, UrlHelper.SiteGroupSetting.SiteGroup, IPHelper.ClientIP());
        private readonly string _totalLoginFailedSessionKey = string.Format(SessionConstants.TOTALLOGINFAILEDCOUNT, UrlHelper.SiteGroupSetting.SiteGroup, IPHelper.ClientIP());
        private readonly string _currentLoginFailedSessionKey = string.Format(SessionConstants.CURRENTLOGINFAILEDCOUNT, UrlHelper.SiteGroupSetting.SiteGroup, IPHelper.ClientIP());
        private readonly string _latestLoginFailSessionKey = string.Format(SessionConstants.LOGINFAILEDTIME, UrlHelper.SiteGroupSetting.SiteGroup, IPHelper.ClientIP());

        private readonly Lazy<string> getLastLoginUsernameLazy;
        private readonly Lazy<int> getTotallyLoginFailed;
        private readonly Lazy<int> getCurrentLoginFailed;
        private readonly Lazy<DateTime> getLoginFailedTime;

        public CacheService()
        {
            getLastLoginUsernameLazy = new Lazy<string>(() => CacheExtension.Get(_lastLoginUserNameSessionKey, string.Empty));
            getTotallyLoginFailed = new Lazy<int>(() => CacheExtension.Get(_totalLoginFailedSessionKey, 0));
            getCurrentLoginFailed = new Lazy<int>(() => CacheExtension.Get(_currentLoginFailedSessionKey, 0));
            getLoginFailedTime = new Lazy<DateTime>(() => CacheExtension.Get(_latestLoginFailSessionKey, DateTime.MinValue));
        }

        public string GetLastLoginUsername()
        {
            return getLastLoginUsernameLazy.Value;
        }

        public void SetLastLoginUsername(string username)
        {
            CacheExtension.Set(_lastLoginUserNameSessionKey, username);
        }

        public int GetTotallyLoginFailed()
        {
            return getTotallyLoginFailed.Value;
        }

        public int GetCurrentLoginFailed()
        {
            return getCurrentLoginFailed.Value;
        }

        public DateTime GetLoginFailedTime()
        {
            return getLoginFailedTime.Value;
        }

        public void SetLoginFailedTime()
        {
            CacheExtension.Set(_latestLoginFailSessionKey, DateTime.UtcNow);
        }

        public void ResetLoginFailed()
        {
            CacheExtension.Remove(_totalLoginFailedSessionKey);
            CacheExtension.Remove(_currentLoginFailedSessionKey);
            CacheExtension.Remove(_latestLoginFailSessionKey);
        }

        public void ResetCurrentLoginFailed()
        {
            CacheExtension.Remove(_currentLoginFailedSessionKey);
        }

        public int IncreaseLoginFailedCount()
        {
            var loginFailCount = GetTotallyLoginFailed() + 1;
            CacheExtension.Set(_totalLoginFailedSessionKey, loginFailCount);
            var currentLoginFailCount = GetCurrentLoginFailed() + 1;
            CacheExtension.Set(_currentLoginFailedSessionKey, currentLoginFailCount);
            return loginFailCount;
        }
    }
}
