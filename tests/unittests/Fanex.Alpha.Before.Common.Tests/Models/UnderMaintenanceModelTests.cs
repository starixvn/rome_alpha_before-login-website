namespace Fanex.Alpha.Before.Common.Tests.Models
{
    using System;
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

        [TestCase("09:00 AM")]
        [TestCase("9:2 AM")]
        [TestCase("09:00 PM")]
        [TestCase("9:00 PM")]
        [TestCase("12:00 PM")]
        [TestCase("11:00 PM")]
        [TestCase("15:00 PM")]
        [TestCase("12:11 PM")]
        [TestCase("1:1 PM")]
        public void ShowCountDown_ToTimeValid_ReturnTrue(string toTime)
        {
            // Arrange
            var utcNow = new DateTime(1989, 5, 28);
            var underMaintenance = new UnderMaintenanceModel(false, "00:00", toTime, false, utcNow);

            // Act
            var isShowCountDown = underMaintenance.ShowCountDown();

            // Assert
            Assert.IsTrue(isShowCountDown);
        }

        [TestCase("09:00 AM", 542)]
        [TestCase("9:2 AM", 544)]
        [TestCase("09:00 PM", 1262)]
        [TestCase("9:01 PM", 1263)]
        [TestCase("12:01 PM", 723)]
        [TestCase("11:11 PM", 1393)]
        [TestCase("15:03 PM", 905)]
        [TestCase("16:11 PM", 973)]
        [TestCase("1:1 PM", 783)]
        public void ShowCountDown_ShowCountDown_UpdateCountDownTime(string toTime, int totalMinutes)
        {
            // Arrange
            var utcNow = new DateTime(1989, 5, 28, 16, 0, 0); // => GMT+8 = 00:00
            var underMaintenance = new UnderMaintenanceModel(false, "00:00", toTime, false, utcNow);

            // Act
            underMaintenance.ShowCountDown();

            // Assert
            Assert.AreEqual(totalMinutes, underMaintenance.CountdownTime.TotalMinutes);
        }
    }
}