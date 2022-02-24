namespace Fanex.Alpha.Before.Common.Tests.Controllers
{
    using System.Web.Mvc;
    using Common.Controllers;
    using Common.Services.LimitedCountryAccessService;
    using Fanex.Logging;
    using Fanex.Logging.RabbitMQ;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class LimitedCountryAccessControllerTests
    {
        private LimitedCountryAccessController limitedCountryAccessController;
        private ILimitedCountryAccessService limitedCountryAccessService;

        [SetUp]
        public void Init()
        {
            LogManager
                .SetDefaultLogCategory("Alpha-MBC-BF-Local")
                .Use(new RabbitMQLogging("host=ha.nexdev.net:5672;virtualHost=/Fanex_Logging;username=fanexlogging;password=YKQrMtla7oYZGlP8apPtSw=="));

            limitedCountryAccessService = Substitute.For<ILimitedCountryAccessService>();
            limitedCountryAccessController = new LimitedCountryAccessController(limitedCountryAccessService);
        }

        [Test]
        public void Index_Always_ReturnCorrectView()
        {
            // Act
            var actualView = (ViewResult)limitedCountryAccessController.IndexView();

            // Assert
            Assert.AreEqual("~/Views/Common/LimitedCountryAccess/Index.cshtml", actualView.ViewName);
        }
    }
}