namespace Fanex.Alpha.Before.Common.Tests.Helpers
{
    using Common.Helpers;
    using NUnit.Framework;

    [TestFixture]
    public class LanguageHelperTests
    {
        [Test]
        public void DefaultLanguage_Always_ReturnEnglish()
        {
            // Act
            var actualLanguage = LanguageHelper.DefaultLanguage;

            // Assert
            Assert.AreEqual("en-US", actualLanguage);
        }
    }
}
