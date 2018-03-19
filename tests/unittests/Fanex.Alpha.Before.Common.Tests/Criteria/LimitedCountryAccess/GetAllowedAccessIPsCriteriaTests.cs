namespace Fanex.Alpha.Before.Common.Tests.Criteria.LimitedCountryAccess
{
    using Common.Criteria.LimitedCountryAccess;
    using NUnit.Framework;

    [TestFixture]
    public class GetAllowedAccessIPsCriteriaTests
    {
        [Test]
        public void GetSettingKey_Always_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetAllowedAccessIPsCriteria();
            var expectedSettingKey = "Age_AllowedAccessIPsSel";

            // Act
            var actualSettingKey = criteria.GetSettingKey();

            // Assert
            Assert.AreEqual(expectedSettingKey, actualSettingKey);
        }
    }
}
