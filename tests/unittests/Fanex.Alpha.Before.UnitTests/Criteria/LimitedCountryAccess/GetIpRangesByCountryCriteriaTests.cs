using Fanex.Alpha.Before.Criteria.LimitedCountryAccess;

namespace Fanex.Alpha.Before.UnitTests.Criteria.LimitedCountryAccess
{
    public class GetIpRangesByCountryCriteriaTests
    {
        [Theory]
        [InlineData(false, "")]
        [InlineData(false, "HKK")]
        [InlineData(true, "")]
        [InlineData(true, "HKK")]
        public void GetSettingKey_Always_ReturnCorrectSettingKey(bool isIpV6, string countryCodes)
        {
            // Arrange
            var criteria = new GetIpRangesByCountryCriteria(isIpV6, countryCodes);
            var expectedSettingKey = "Age_IPRangesByCountrySel";

            // Act
            var actualSettingKey = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expectedSettingKey, actualSettingKey);
        }
    }
}
