using Fanex.Alpha.Before.Configurations;

namespace Fanex.Alpha.Before.Models
{
    public class SignInResult
    {
        public int ErrCode
        {
            get;
            set;
        }

        public string ErrMsg
        {
            get;
            set;
        }

        public int Language
        {
            get;
            set;
        }

        public string Ip
        {
            get;
            set;
        }

        public string UTMS
        {
            get;
            set;
        }

        public string Domain
        {
            get;
            set;
        }

        public string Captcha
        {
            get;
            set;
        }

        public bool IsChangedPassword
        {
            get;
            set;
        }

        public string ChangedPasswordTime
        {
            get;
            set;
        }

        public string AfterLoginUrl
        {
            get;
            set;
        }

        public string LoginTemplate
        {
            get;
            set;
        }

        public string Username
        {
            get;
            set;
        }

        public bool IsCaptchaVerified { get; set; }

        public string AssetAppPaths { get; } = AppSetting.AssetAppPaths;

        public DateTime LoginTime { get; set; }

        public int NoInteractionTimeInMinutes { get; set; }
        public long CustId { get; set; }


        public long CustSubId { get; set; }
    }
}
