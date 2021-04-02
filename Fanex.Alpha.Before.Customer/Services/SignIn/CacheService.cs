namespace Fanex.Alpha.Before.Customer.Services.SignIn
{
    using Fanex.Alpha.Before.Common.Constants;
    using Fanex.Alpha.Before.Common.Helpers;
    using Fanex.Alpha.Before.Infrastructure.Extensions;
    using System;

    public interface ICacheService
    {
        int GetTotallyLoginFailed();

        int GetCurrentLoginFailed(string username);

        DateTime GetLoginFailedTime();

        void SetLoginFailedTime();

        void ResetLoginFailed(string username);

        void ResetCurrentLoginFailed(string username);

        int IncreaseLoginFailedCount(string username);
    }

    public class CacheService : ICacheService
    {
        private readonly string _totalLoginFailedSessionKey = string.Format(SessionConstants.TOTALLOGINFAILEDCOUNT, UrlHelper.SiteGroupSetting.SiteGroup, IPHelper.ClientIP());
        private readonly string _latestLoginFailSessionKey = string.Format(SessionConstants.LOGINFAILEDTIME, UrlHelper.SiteGroupSetting.SiteGroup, IPHelper.ClientIP());

        private static string GetCurrentLoginFailedSessionKey(string username)
        {
            return string.Format(SessionConstants.CURRENTLOGINFAILEDCOUNT, UrlHelper.SiteGroupSetting.SiteGroup, IPHelper.ClientIP(), username);
        }

        public int GetTotallyLoginFailed()
        {
            return CacheExtension.Get(_totalLoginFailedSessionKey, 0);
        }

        public int GetCurrentLoginFailed(string username)
        {
            return CacheExtension.Get(GetCurrentLoginFailedSessionKey(username), 0);
        }

        public DateTime GetLoginFailedTime()
        {
            return CacheExtension.Get(_latestLoginFailSessionKey, DateTime.MinValue);
        }

        public void SetLoginFailedTime()
        {
            CacheExtension.Set(_latestLoginFailSessionKey, DateTime.UtcNow);
        }

        public void ResetLoginFailed(string username)
        {
            CacheExtension.Remove(_totalLoginFailedSessionKey);
            CacheExtension.Remove(GetCurrentLoginFailedSessionKey(username));
            CacheExtension.Remove(_latestLoginFailSessionKey);
        }

        public void ResetCurrentLoginFailed(string username)
        {
            CacheExtension.Remove(GetCurrentLoginFailedSessionKey(username));
        }

        public int IncreaseLoginFailedCount(string username)
        {
            var loginFailCount = GetTotallyLoginFailed() + 1;
            CacheExtension.Set(_totalLoginFailedSessionKey, loginFailCount);
            var currentLoginFailCount = GetCurrentLoginFailed(username) + 1;
            CacheExtension.Set(GetCurrentLoginFailedSessionKey(username), currentLoginFailCount);
            return loginFailCount;
        }
    }
}
