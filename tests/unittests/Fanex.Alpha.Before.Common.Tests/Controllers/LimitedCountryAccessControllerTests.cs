namespace Fanex.Alpha.Before.Common.Tests.Controllers
{
    using System.Web.Mvc;
    using Common.Controllers;
    using Common.Services.LimitedCountryAccessService;
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