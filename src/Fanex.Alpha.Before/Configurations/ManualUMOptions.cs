namespace Fanex.Alpha.Before.Configurations;

public class ManualUMOptions
{
    public const string ManualUM = "ManualUM";

    public string[] Sites { get; set; } = Array.Empty<string>();

    public string UMFrom { get; set; }

    public string UMTo { get; set; }
}