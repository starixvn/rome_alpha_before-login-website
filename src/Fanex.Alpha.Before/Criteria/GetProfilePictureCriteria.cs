using Fanex.Data.Repository;

namespace Fanex.Alpha.Before.Criteria
{
    public class GetProfilePictureCriteria : CriteriaBase
    {
        public GetProfilePictureCriteria(int custId)
        {
            CustId = custId;
        }

        public int CustId { get; }

        public override string GetSettingKey() => "Age_CustomerSelProfilePicture";

        public override bool IsValid() => CustId > 0;
    }
}