namespace Fanex.Alpha.Before.Common.Models
{
    public class IpRange
    {
        public IpRange(ulong ipFrom, ulong ipTo, string countryCode)
        {
            IPFrom = ipFrom;
            IPTo = ipTo;
            CountryCode = countryCode;
        }

        public ulong IPFrom { get; set; }

        public ulong IPTo { get; set; }

        public string CountryCode { get; set; }
    }
}
