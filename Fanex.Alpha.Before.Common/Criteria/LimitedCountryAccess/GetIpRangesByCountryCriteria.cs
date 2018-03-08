namespace Fanex.Alpha.Before.Common.Criteria.LimitedCountryAccess
{
    using Data.Repository;

    public class GetIpRangesByCountryCriteria : CriteriaBase
    {
        public GetIpRangesByCountryCriteria(string countryCodes)
        {
            CountryCodes = countryCodes;
        }

        public string CountryCodes { get; }

        public override string GetSettingKey() => "Age_IPRangesByCountrySel";
    }
}
