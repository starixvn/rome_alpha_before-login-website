namespace Fanex.Alpha.Before.Criteria.LimitedCountryAccess
{
    using Data.Repository;

    public class GetIpRangesByCountryCriteria : CriteriaBase
    {
        public GetIpRangesByCountryCriteria(bool isIpV6, string countryCodes)
        {
            IsIpV6 = isIpV6;
            CountryCodes = countryCodes;
        }

        public bool IsIpV6 { get; }

        public string CountryCodes { get; }

        public override string GetSettingKey() => "Age_IPRangesByCountrySel";

        public override bool IsValid() => true;
    }
}
