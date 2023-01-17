namespace Fanex.Alpha.Before.NetThrottling.Models;

[Serializable]
public class ThrottleCounter
{
    public DateTime Timestamp { get; set; }

    public int TotalRequests { get; set; }
}