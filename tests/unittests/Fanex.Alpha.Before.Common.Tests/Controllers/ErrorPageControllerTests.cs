namespace Fanex.Alpha.Before.Common.Tests.Controllers
{
    using System.Web.Mvc;
    using Common.Controllers;
    using Fanex.Logging;
    using Fanex.Logging.RabbitMQ;
    using NUnit.Framework;

    [TestFixture]
    public class ErrorPageControllerTests
    {
        private ErrorPageController errorPageController;

        [SetUp]
        public void Init()
        {
            LogManager
                .SetDefaultLogCategory("Alpha-MBC-BF-Local")
                .Use(new RabbitMQLogging("host=ha.nexdev.net:5672;virtualHost=/Fanex_Logging;username=fanexlogging;password=YKQrMtla7oYZGlP8apPtSw=="));

            errorPageController = new ErrorPageController();
        }

        [Test]
        public void Index_Always_ReturnCorrectView()
        {
            // Act
            var actualView = (ViewResult)errorPageController.Index();

            // Assert
            Assert.AreEqual("~/Views/Common/ErrorPage/Index.cshtml", actualView.ViewName);
        }
    }
}
