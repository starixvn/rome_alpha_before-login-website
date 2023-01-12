namespace Fanex.Alpha.Before.Configurations;

public class RobotOptions
{
    public const string Robot = "Robot";

    public bool EnableDetectBot { get; set; }

    public bool IsRobotAlways { get; set; }

    public int[] ExcludeSuperIds { get; set; } = Array.Empty<int>();

    public string[] ExcludeSites { get; set; } = Array.Empty<string>();

    public string FPSBotDetectionApiUrl { get; set; } = string.Empty;
}