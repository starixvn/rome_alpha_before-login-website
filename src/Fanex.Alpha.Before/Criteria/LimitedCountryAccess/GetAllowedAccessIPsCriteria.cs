namespace Fanex.Alpha.Before.Criteria.LimitedCountryAccess
{
    using Data.Repository;

    public class GetAllowedAccessIPsCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Age_AllowedAccessIPsSel";

        public override bool IsValid() => true;
    }
}
