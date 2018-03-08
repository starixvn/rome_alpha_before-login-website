namespace Fanex.Alpha.Before.Common.Models
{
    public class IpRange
    {
        public IpRange(long ipFrom, long ipTo, string countryCode)
        {
            IPFrom = ipFrom;
            IPTo = ipTo;
            CountryCode = countryCode;
        }

        public long IPFrom { get; set; }

        public long IPTo { get; set; }

        public string CountryCode { get; set; }
    }
}
