namespace Fanex.Alpha.Before.Customer.Services.SignIn
{
    using System;
    using Fanex.Alpha.Before.Common.Constants;
    using Fanex.Alpha.Before.Common.Helpers;
    using Fanex.Alpha.Before.Infrastructure.Extensions;
    using Fanex.Caching;

    public interface ICacheService
    {
        bool IsLoginIPBlocked();

        void SetLoginIPBlocked(int blockLoginMinute);

        int GetLoginIPFailedCount();

        int GetLoginUserFailedCount(string username);

        void ResetLoginSession(string username);

        int IncreaseLoginIPFailedCount();

        int IncreaseLoginUserFailedCount(string username, int notInteractionTimeInMinutes);
    }

    public class CacheService : ICacheService
    {
        private readonly string _loginIPFailedCountSessionKey = string.Format(SessionConstants.LOGINIPFAILEDCOUNT, UrlHelper.SiteGroupSetting.SiteGroup, IPHelper.ClientIP());
        private readonly string _loginIPBlockedSessionKey = string.Format(SessionConstants.LOGINIPBLOCKED, UrlHelper.SiteGroupSetting.SiteGroup, IPHelper.ClientIP());

        private static string GetLoginUserFailedCountSessionKey(string username)
        {
            return string.Format(SessionConstants.LOGINUSERFAILEDCOUNT, UrlHelper.SiteGroupSetting.SiteGroup, IPHelper.ClientIP(), username);
        }

        public bool IsLoginIPBlocked()
        {
            return CacheExtension.Contain(_loginIPBlockedSessionKey);
        }

        public void SetLoginIPBlocked(int blockLoginMinute)
        {
            CacheExtension.Remove(_loginIPFailedCountSessionKey);
            CacheExtension.Set(_loginIPBlockedSessionKey, DateTime.UtcNow, new CacheItemOptions
            {
                AbsoluteExpiration = DateTime.UtcNow.AddMinutes(blockLoginMinute)
            });
        }

        public int GetLoginIPFailedCount()
        {
            return CacheExtension.Get(_loginIPFailedCountSessionKey, 0);
        }

        public int GetLoginUserFailedCount(string username)
        {
            return CacheExtension.Get(GetLoginUserFailedCountSessionKey(username), 0);
        }

        public void ResetLoginSession(string username)
        {
            CacheExtension.Remove(_loginIPFailedCountSessionKey);
            CacheExtension.Remove(_loginIPBlockedSessionKey);
            CacheExtension.Remove(GetLoginUserFailedCountSessionKey(username));
        }

        public int IncreaseLoginIPFailedCount()
        {
            var loginIPFailedCount = GetLoginIPFailedCount() + 1;
            CacheExtension.Set(_loginIPFailedCountSessionKey, loginIPFailedCount);
            return loginIPFailedCount;
        }

        public int IncreaseLoginUserFailedCount(string username, int notInteractionTimeInMinutes)
        {
            var loginUserFailedCount = GetLoginUserFailedCount(username) + 1;
            CacheExtension.Set(GetLoginUserFailedCountSessionKey(username), loginUserFailedCount, TimeSpan.FromMinutes(notInteractionTimeInMinutes));
            return loginUserFailedCount;
        }
    }
}
