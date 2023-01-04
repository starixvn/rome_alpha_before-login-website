namespace Fanex.Alpha.Before.Customer.Configurations;

public class CommonOptions
{
    public const string Common = "Common";

    public string[] OfficeIPs { get; set; }

    public int MaxTimesToAllowLoginFail { get; set; }

    public int BlockLoginMinutes { get; set; }

    public int LoginFailTimeToStartUsingCaptcha { get; set; }

    public int PasswordMaxFailTimeForOneUser { get; set; }

    public int NoInteractionTimeInMinutes { get; set; }
}