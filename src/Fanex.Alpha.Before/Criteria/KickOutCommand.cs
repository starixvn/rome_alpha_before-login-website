using Fanex.Data.Repository;

namespace Fanex.Alpha.Before.Criteria
{
    public class KickOutCommand : NonQueryCommand
    {
        public KickOutCommand(int custId, byte roleId, int serverId, string ip, int subAccId, string mode)
        {
            CustId = custId;
            RoleId = roleId;
            ServerId = serverId;
            IP = ip;
            SubAccId = subAccId;
            Mode = mode;
        }

        public int CustId { get; }

        public byte RoleId { get; }

        public int ServerId { get; }

        public string IP { get; }

        public int SubAccId { get; }

        public string Mode { get; }

        public override string GetSettingKey() => "Age_OnUser_ForceKickedout";

        public override bool IsValid() => true;
    }
}