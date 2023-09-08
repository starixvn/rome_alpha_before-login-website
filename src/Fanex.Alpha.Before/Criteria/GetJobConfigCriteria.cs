using Fanex.Data.Repository;

namespace Fanex.Alpha.Before.Criteria;

public class GetJobConfigCriteria : CriteriaBase
{
    public GetJobConfigCriteria() { }

    public override string GetSettingKey()
    {
        return "Age_ClearBalanceCust_Job_ConfigSel";
    }

    public override bool IsValid() => true;
}
