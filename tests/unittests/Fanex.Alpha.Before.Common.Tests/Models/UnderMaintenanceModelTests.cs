namespace Fanex.Alpha.Before.Common.Tests.Models
{
    using Common.Models;
    using NUnit.Framework;

    [TestFixture]
    public class UnderMaintenanceModelTests
    {
        [TestCase("1", "2")]
        [TestCase("dfdf", "dfdf")]
        [TestCase("", "")]
        [TestCase("12", "")]
        [TestCase("dfdf", "12")]
        [TestCase("", "12")]
        public void ShowCountDown_HourOrMinuteIsNotValid_ReturnFalse(string hours, string minutes)
        {
            // Arrange
            var underMaintenance = new UnderMaintenanceModel(false, "00:00", $"{hours}:{minutes}", false);

            // Act
            var isShowCountDown = underMaintenance.ShowCountDown();

            // Assert
            Assert.IsFalse(isShowCountDown);
        }
    }
}