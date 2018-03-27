namespace Fanex.Alpha.Before.Common.Tests.Services
{
    using Common.Models;
    using Common.Services.UnderMainternance;
    using KellermanSoftware.CompareNetObjects;
    using NSubstitute;
    using NUnit.Framework;
    using ONEbook.UM.Models;
    using ONEbook.UM.Services;

    [TestFixture]
    public class UnderMainternanceServiceTests
    {
        private UnderMainternanceService underMainternanceService;
        private IMaintenanceService maintenanceService;
        private CompareLogic compareLogic;

        [SetUp]
        public void Init()
        {
            maintenanceService = Substitute.For<IMaintenanceService>();
            underMainternanceService = new UnderMainternanceService(maintenanceService);
            compareLogic = new CompareLogic();
        }

        [Test]
        public void UnderMainternanceInfo_Always_ReturnCorrectInfo()
        {
            // Arrange
            maintenanceService.GetUnderMaintenanceTime()
                .Returns(new UnderMaintenanceTime(false, string.Empty, string.Empty, null));

            maintenanceService.IsUnderMaintenance().Returns(false);
            var expectedInfo = new UnderMaintenanceModel(false, string.Empty, string.Empty, false, null);

            // Act
            var actualInfo = underMainternanceService.UnderMainternanceInfo();

            // Assert
            Assert.IsTrue(compareLogic.Compare(expectedInfo, actualInfo).AreEqual);
        }
    }
}
