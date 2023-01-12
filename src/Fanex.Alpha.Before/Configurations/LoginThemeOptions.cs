namespace Fanex.Alpha.Before.Configurations;

public class LoginThemeOptions
{
    public const string LoginTheme = "LoginTheme";

    public string DefaultTheme { get; set; } = "default";

    public string EventTheme { get; set; } = "event";

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }
}