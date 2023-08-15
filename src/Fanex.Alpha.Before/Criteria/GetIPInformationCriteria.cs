namespace Fanex.Alpha.Before.Criteria
{
    using System.Net;
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
            => !string.IsNullOrEmpty(Ip) && IPAddress.TryParse(Ip, out _);
    }
}
