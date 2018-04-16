using Fanex.Alpha.Before.Common.Configurations.UnderMainternance;
using Fanex.Alpha.Before.Common.Models;
using ONEbook.UM.Services;
using ONEbook.UM.Services.Impl;

namespace Fanex.Alpha.Before.Common.Services.UnderMainternance
{
    public class UnderMainternanceService : IUnderMainternanceService
    {
        private readonly IMaintenanceService _maintenanceService;

        public UnderMainternanceService() : this(null)
        {
        }

        public UnderMainternanceService(IMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService ?? new MaintenanceService();
        }

        /// <summary>
        /// Checking UM.
        /// </summary>
        /// <returns>Return UM info.</returns>
        public UnderMaintenanceModel UnderMainternanceInfo()
        {
            var underMaintenanceTime = _maintenanceService.GetUnderMaintenanceTime();

            var result = new UnderMaintenanceModel(
                _maintenanceService.IsUnderMaintenance(),
                underMaintenanceTime.From,
                underMaintenanceTime.To,
                underMaintenanceTime.ConnectionResult.IsNotOk);

            return result;
        }
    }
}