namespace Fanex.Alpha.Before.Common.Tests.Controllers
{
    using System.Web.Mvc;
    using Common.Controllers;
    using NUnit.Framework;

    [TestFixture]
    public class LimitedCountryAccessControllerTests
    {
        private LimitedCountryAccessController limitedCountryAccessController;

        [SetUp]
        public void Init()
        {
            limitedCountryAccessController = new LimitedCountryAccessController();
        }

        [Test]
        public void Index_Always_ReturnCorrectView()
        {
            // Act
            var actualView = (ViewResult)limitedCountryAccessController.Index();

            // Assert
            Assert.AreEqual("~/Views/Common/LimitedCountryAccess/Index.cshtml", actualView.ViewName);
        }
    }
}
