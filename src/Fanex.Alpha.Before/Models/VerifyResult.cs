namespace Fanex.Alpha.Before.Models
{
    public class VerifyResult
    {
        public VerifyResult(int code, string message, string avatar)
        {
            Code = code;
            Message = message;
            Avatar = avatar;
        }

        public int Code { get; }

        public string Message { get; }

        public string Avatar { get; }

        public bool IsSuccess => Code == 0;

        public static VerifyResult CreateErrorResult(string message, int code = 1) => new(code, message, string.Empty);

        public static VerifyResult CreateSuccessResult(string avatar) => new(0, string.Empty, avatar);
    }
}