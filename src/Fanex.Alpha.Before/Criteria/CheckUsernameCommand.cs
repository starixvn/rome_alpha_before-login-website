using Fanex.Alpha.Before.Configurations;
using Fanex.Data.Repository;

namespace Fanex.Alpha.Before.Criteria
{
    public class CheckUsernameCommand : NonQueryCommand
    {
        public CheckUsernameCommand(string username, string loginIP, string loginDomain)
        {
            Username = username;
            LoginIP = loginIP;
            LoginDomain = loginDomain;
        }

        public string Username { get; }

        public string LoginIP { get; }

        public string LoginDomain { get; }

        public int CustId { get; set; }

        public string SiteGroup => AppSetting.SiteGroup;

        public override string GetSettingKey() => "Age_LoginCheckUsername";

        public override bool IsValid() => !string.IsNullOrEmpty(Username);
    }
}