namespace Fanex.Alpha.Before.Customer.Models
{
    public class SignInModel
    {
        public string Username { get; }

        public string Password { get; }

        public string Language { get; }

        public string Code { get; }

        public string CaptchaToken { get; }

        public string BrowserSize { get; }

        public string DeviceId { get; }

        public SignInModel(string username, string password, string language, string code, string captchaToken, string browserSize, string deviceId)
        {
            Username = username;
            Password = password;
            Language = language;
            Code = code;
            CaptchaToken = captchaToken;
            BrowserSize = browserSize;
            DeviceId = deviceId;
        }
    }
}
