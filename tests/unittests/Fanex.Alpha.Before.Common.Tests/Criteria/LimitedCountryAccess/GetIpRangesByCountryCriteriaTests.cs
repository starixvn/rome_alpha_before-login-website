namespace Fanex.Alpha.Before.Common.Tests.Criteria.LimitedCountryAccess
{
    using Common.Criteria.LimitedCountryAccess;
    using NUnit.Framework;

    [TestFixture]
    public class GetIpRangesByCountryCriteriaTests
    {
        [TestCase(false, "")]
        [TestCase(false, "HKK")]
        [TestCase(true, "")]
        [TestCase(true, "HKK")]
        public void GetSettingKey_Always_ReturnCorrectSettingKey(bool isIpV6, string countryCodes)
        {
            // Arrange
            var criteria = new GetIpRangesByCountryCriteria(isIpV6, countryCodes);
            var expectedSettingKey = "Age_IPRangesByCountrySel";

            // Act
            var actualSettingKey = criteria.GetSettingKey();

            // Assert
            Assert.AreEqual(expectedSettingKey, actualSettingKey);
        }
    }
}
