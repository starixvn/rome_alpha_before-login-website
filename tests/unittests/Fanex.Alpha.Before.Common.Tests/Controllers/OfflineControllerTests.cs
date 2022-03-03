namespace Fanex.Alpha.Before.Common.Tests.Controllers
{
    using System.Web.Mvc;
    using Common.Controllers;
    using Common.Models;
    using Configurations.UnderMainternance;
    using Fanex.Logging;
    using Fanex.Logging.RabbitMQ;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class OfflineControllerTests
    {
        private OfflineController offlineController;
        private IUnderMainternanceService underMainternanceService;

        [SetUp]
        public void Init()
        {
            LogManager
                .SetDefaultLogCategory("Alpha-MBC-BF-Local")
                .Use(new RabbitMQLogging("host=ha.nexdev.net:5672;virtualHost=/Fanex_Logging;username=fanexlogging;password=YKQrMtla7oYZGlP8apPtSw=="));

            underMainternanceService = Substitute.For<IUnderMainternanceService>();

            offlineController = new OfflineController(underMainternanceService);
        }

        [Test]
        public void Index_IsUM_ReturnUnderMaintenanceView()
        {
            // Arrange
            underMainternanceService.UnderMainternanceInfo()
                .Returns(new UnderMaintenanceModel(true, string.Empty, string.Empty, false));

            // Act
            var actualView = (ViewResult)offlineController.Index();

            // Assert
            Assert.AreEqual("~/Views/Common/UnderMaintenance/Index.cshtml", actualView.ViewName);
        }
    }
}
