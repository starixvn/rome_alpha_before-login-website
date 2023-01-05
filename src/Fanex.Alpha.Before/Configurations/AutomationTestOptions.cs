namespace Fanex.Alpha.Before.Configurations;

public class AutomationTestOptions
{
    public const string AutomationTest = "AutomationTest";

    public string[] AllowedIPs { get; set; } = Array.Empty<string>();

    public string Captcha { get; set; } = string.Empty;

    public string[] AllowUsernameContainWords { get; set; } = Array.Empty<string>();
}