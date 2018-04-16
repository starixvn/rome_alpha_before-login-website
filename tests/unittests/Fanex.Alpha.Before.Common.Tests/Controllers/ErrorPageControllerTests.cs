namespace Fanex.Alpha.Before.Common.Tests.Controllers
{
    using System.Web.Mvc;
    using Common.Controllers;
    using NUnit.Framework;

    [TestFixture]
    public class ErrorPageControllerTests
    {
        private ErrorPageController errorPageController;

        [SetUp]
        public void Init()
        {
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
