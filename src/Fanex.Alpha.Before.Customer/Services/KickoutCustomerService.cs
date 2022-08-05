namespace Fanex.Alpha.Before.Customer.Services
{
    using System;
    using Fanex.Alpha.Before.Customer.Repositories;
    using Fanex.Logging.Extensions.Web;

    public interface IKickoutCustomerService
    {
        void KickoutDueToProtectedMode(int custId, byte roleId, int serverId, string ip, int subAccId);

        void Kickout(int custId, byte roleId, int serverId, string ip, int subAccId, string mode);
    }

    public class KickoutCustomerService : IKickoutCustomerService
    {
        private readonly IKickoutCustomerRepository repository;

        public KickoutCustomerService(IKickoutCustomerRepository repository = null)
        {
            this.repository = repository ?? new KickoutCustomerRepository();
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
                repository.Kickout(custId, roleId, serverId, ip, subAccId, mode);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }
        }
    }
}
