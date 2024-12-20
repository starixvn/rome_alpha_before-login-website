using Fanex.Alpha.Before.Configurations;
using Fanex.Data.Repository;

namespace Fanex.Alpha.Before.Criteria
{
    public class GetRandomProfilePictureCriteria : CriteriaBase
    {

        public int Mode => AppSetting.Is789ySite ? 1 : 2;

        public bool IsRandom => true;

        public override string GetSettingKey() => "Age_CustomerProfilePictureLibrarySel";

        public override bool IsValid() => true;
    }
}