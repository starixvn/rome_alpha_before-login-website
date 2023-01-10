using Fanex.Alpha.Before.Helpers;

namespace Fanex.Alpha.Before.UnitTests.Helpers
{
    public class LanguageHelperTests
    {
        [Fact]
        public void DefaultLanguage_Always_ReturnEnglish()
        {
            // Act
            var actualLanguage = LanguageHelper.DefaultLanguage;

            // Assert
            Assert.Equal("en-US", actualLanguage);
        }
    }
}
