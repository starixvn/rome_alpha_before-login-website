namespace Fanex.Alpha.Before.Models
{
    public record IpRange
    {
        public ulong IPFrom { get; init; }

        public ulong IPTo { get; init; }

        public string CountryCode { get; init; }
    }
}
