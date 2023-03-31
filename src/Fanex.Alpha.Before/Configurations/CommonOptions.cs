namespace Fanex.Alpha.Before.Configurations;

public class CommonOptions
{
    public const string Common = "Common";

    public int LoginFailTimeToStartUsingCaptcha { get; set; }

    public int NoInteractionTimeInMinutes { get; set; }
}