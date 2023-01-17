namespace Fanex.Alpha.Before.NetThrottling;

public class ThrottlePolicy
{
    public string Name { get; set; } = string.Empty;

    public int MaxRequests { get; set; }

    public int ThrottleDuration { get; set; }

    public int BlockDuration { get; set; }

    public int[] WhitelistSuperIds { get; set; } = Array.Empty<int>();
}