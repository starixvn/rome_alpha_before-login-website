namespace Fanex.Alpha.Before.Common.Criteria
{
    using Data.Repository;

    public class GetIPInformationCriteria : CriteriaBase
    {
        public GetIPInformationCriteria(string ip)
        {
            Ip = ip;
        }

        public string Ip { get; set; }

        public override string GetSettingKey()
            => "Acc_GetIPInfo_Full";

        public override bool IsValid()
            => !string.IsNullOrEmpty(Ip);
    }
}
