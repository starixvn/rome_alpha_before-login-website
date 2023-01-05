using Fanex.Alpha.Before.Criteria;
using Fanex.Data.Repository;
using Fanex.Logging;

namespace Fanex.Alpha.Before.Services
{
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
