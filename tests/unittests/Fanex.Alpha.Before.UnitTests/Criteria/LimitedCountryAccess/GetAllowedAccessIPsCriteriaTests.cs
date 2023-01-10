using Fanex.Alpha.Before.Criteria.LimitedCountryAccess;

namespace Fanex.Alpha.Before.UnitTests.Criteria.LimitedCountryAccess
{

    public class GetAllowedAccessIPsCriteriaTests
    {
        [Fact]
        public void GetSettingKey_Always_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetAllowedAccessIPsCriteria();

            // Act & Assert
            Assert.Equal("Age_AllowedAccessIPsSel", criteria.GetSettingKey());
        }

        [Fact]
        public void IsValid_Always_ReturnTrue()
        {
            // Arrange
            var criteria = new GetAllowedAccessIPsCriteria();

            // Act & Assert
            Assert.True(criteria.IsValid());
        }
    }
}
