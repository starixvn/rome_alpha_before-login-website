namespace Fanex.Alpha.Before.Common.Models
{
    public class GoogleReCaptcha
    {
        public GoogleReCaptcha()
        {
        }

        public GoogleReCaptcha(string siteKey, string secretKey, string proxy)
        {
            SiteKey = siteKey;
            SecretKey = secretKey;
            Proxy = proxy;
            IsEnable = false;// !string.IsNullOrWhiteSpace(SiteKey) && !string.IsNullOrWhiteSpace(SecretKey);
        }

        public string SiteKey { get; }

        public string SecretKey { get; }

        public string Proxy { get; }

        public bool IsEnable { get; }
    }
}
