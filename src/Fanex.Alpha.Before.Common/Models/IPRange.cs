namespace Fanex.Alpha.Before.Common.Models
{
    public record IpRange
    {
        public ulong IPFrom { get; init; }

        public ulong IPTo { get; init; }

        public string CountryCode { get; init; }
    }
}
