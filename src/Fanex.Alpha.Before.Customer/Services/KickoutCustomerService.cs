namespace Fanex.Alpha.Before.Customer.Services
{
    using System;
    using Fanex.Alpha.Before.Customer.Criteria;
    using Fanex.Data.Repository;
    using Fanex.Logging;

    public interface IKickoutCustomerService
    {
        void KickoutDueToProtectedMode(int custId, byte roleId, int serverId, string ip, int subAccId);

        void Kickout(int custId, byte roleId, int serverId, string ip, int subAccId, string mode);
    }

    public class KickoutCustomerService : IKickoutCustomerService
    {
        private readonly IDynamicRepository repository;

        public KickoutCustomerService(IDynamicRepository repository)
        {
            this.repository = repository;
        }

        public void KickoutDueToProtectedMode(int custId, byte roleId, int serverId, string ip, int subAccId)
            => Kickout(custId, roleId, serverId, ip, subAccId, "ChangeProtectionModeByLogin");

        // mode
        // ChangePassWord: kickout when upline change password
        // ChangeProtectionMode: kickout when upline update ProtectedStatus = TRUE
        // ChangeProtectionModeByLogin: kickout when main/sub-acc login with Protected Mode
        public void Kickout(int custId, byte roleId, int serverId, string ip, int subAccId, string mode)
        {
            try
            {
                repository.Execute(new KickOutCommand(custId, roleId, serverId, ip, subAccId, mode));
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.Message, ex);
            }
        }
    }
}
