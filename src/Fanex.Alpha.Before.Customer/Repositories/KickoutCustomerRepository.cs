namespace Fanex.Alpha.Before.Customer.Repositories
{
    using System.Collections;
    using Fanex.Data;

    public interface IKickoutCustomerRepository
    {
        void Kickout(int custId, byte roleId, int serverId, string ip, int subAccId, string mode);
    }

    public class KickoutCustomerRepository : IKickoutCustomerRepository
    {
        public void Kickout(int custId, byte roleId, int serverId, string ip, int subAccId, string mode)
        {
            var inputParams = new Hashtable
            {
                ["custId"] = custId,
                ["roleId"] = roleId,
                ["serverId"] = serverId,
                ["ip"] = ip,
                ["subAccId"] = subAccId,
                ["mode"] = mode
            };

            using (var db = new ObjectDb("Age_OnUser_ForceKickedout"))
            {
                db.AssignParamValues(inputParams);
                db.ExecuteNonQuery();
            }
        }
    }
}
