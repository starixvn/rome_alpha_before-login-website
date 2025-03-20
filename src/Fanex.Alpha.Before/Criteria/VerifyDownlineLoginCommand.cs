using Fanex.Alpha.Before.Configurations;
using Fanex.Data.Repository;

namespace Fanex.Alpha.Before.Criteria
{
    public class VerifyDownlineLoginCommand : NonQueryCommand
    {
        public VerifyDownlineLoginCommand(
            string username,
            string userpwd,
            string loginIP,
            string loginDomain,
            string language,
            string browserName,
            string platform,
            string countryCode,
            bool isDarkMode)
        {
            Username = username;
            UserPwd = userpwd;
            LoginIP = loginIP;
            LoginDomain = loginDomain;
            Language = language;
            BrowserName = browserName;
            Platform = platform;
            CountryCode = countryCode;
            IsDarkMode = isDarkMode;
        }

        public string Username { get; set; }

        public string UserPwd { get; set; }

        public int CustId { get; set; }

        public int SubAccId { get; set; }

        public string LoginIP { get; }

        public int ServerId { get; set; }

        public DateTime LoginTime { get; set; } = DateTime.Now;

        public string LoginDomain { get; }

        public string SiteGroup { get; } = AppSetting.SiteGroup;

        public string Language { get; }

        public string BrowserName { get; }

        public string Platform { get; }

        public string CountryCode { get; }

        public DateTime? LastChangedPwdDate { get; set; }

        public bool IsNickname { get; set; }

        public string Nickname { get; set; }

        public int SuperId { get; set; }

        public string Site { get; set; }

        public int SiteId { get; set; }

        public byte RoleId { get; set; }

        public string SubUsername { get; set; }

        public bool IsKickOut { get; set; }

        public bool IsDarkMode { get; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public int FailedCount { get; set; }

        public override string GetSettingKey()
            => "Age_LoginVerifyDownline";

        public override bool IsValid()
            => !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(UserPwd);
    }
}