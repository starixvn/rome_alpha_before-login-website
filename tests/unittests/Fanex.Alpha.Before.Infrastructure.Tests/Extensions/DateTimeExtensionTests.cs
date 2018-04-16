namespace Fanex.Alpha.Before.Infrastructure.Tests.Extensions
{
    using System;
    using Infrastructure.Extensions;
    using NUnit.Framework;

    [TestFixture]
    public class DateTimeExtensionTests
    {
        [Test]
        public void IsValid_LessThanMinDate_ReturnFalse()
        {
            // Arrange
            var dateTime = new DateTime(1899, 1, 1);

            // Act
            var actualIsValid = dateTime.IsValid();

            // Assert
            Assert.IsFalse(actualIsValid);
        }

        [Test]
        public void IsValid_ValidDateTime_ReturnTrue()
        {
            // Arrange
            var dateTime = new DateTime(2018, 3, 28);

            // Act
            var actualIsValid = dateTime.IsValid();

            // Assert
            Assert.IsTrue(actualIsValid);
        }

        [Test]
        public void IsInRange_LessThanFromDate_ReturnFalse()
        {
            // Arrange
            var fromDate = new DateTime(2018, 3, 29);
            var toDate = new DateTime(2018, 3, 30);
            var dateTime = new DateTime(2018, 3, 28);

            // Act
            var actualIsValid = dateTime.IsInRange(fromDate, toDate);

            // Assert
            Assert.IsFalse(actualIsValid);
        }

        [TestCase(29)]
        [TestCase(30)]
        [TestCase(31)]
        public void IsInRange_InRange_ReturnTrue(int date)
        {
            // Arrange
            var fromDate = new DateTime(2018, 3, 29);
            var toDate = new DateTime(2018, 3, 31);
            var dateTime = new DateTime(2018, 3, date);

            // Act
            var actualIsValid = dateTime.IsInRange(fromDate, toDate);

            // Assert
            Assert.IsTrue(actualIsValid);
        }

        [Test]
        public void IsInRange_GreaterThanToDate_ReturnFalse()
        {
            // Arrange
            var fromDate = new DateTime(2018, 3, 29);
            var toDate = new DateTime(2018, 3, 30);
            var dateTime = new DateTime(2018, 3, 31);

            // Act
            var actualIsValid = dateTime.IsInRange(fromDate, toDate);

            // Assert
            Assert.IsFalse(actualIsValid);
        }

        [Test]
        public void ToShortFormatedDate_Always_ReturnCorrectValue()
        {
            // Arrange
            var dateTime = new DateTime(2018, 3, 31);
            var expectedValue = "Mar 31, 2018";

            // Act
            var actualValue = dateTime.ToShortFormatedDate();

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void ToHourMinuteFormated_Always_ReturnCorrectValue()
        {
            // Arrange
            var dateTime = new DateTime(2018, 3, 31, 11, 24, 03);
            var expectedValue = "11:24";

            // Act
            var actualValue = dateTime.ToHourMinuteFormated();

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void ToHourMinuteMiddayTypeFormated_Always_ReturnCorrectValue()
        {
            // Arrange
            var dateTime = new DateTime(2018, 3, 31, 11, 24, 03);
            var expectedValue = "11:24 AM";

            // Act
            var actualValue = dateTime.ToHourMinuteMiddayTypeFormated();

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void ToShortFormatedDateWithSlash_Always_ReturnCorrectValue()
        {
            // Arrange
            var dateTime = new DateTime(2018, 3, 31, 11, 24, 03);
            var expectedValue = "03/31/2018";

            // Act
            var actualValue = dateTime.ToShortFormatedDateWithSlash();

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void ToLongFormatedDateTime_Always_ReturnCorrectValue()
        {
            // Arrange
            var dateTime = new DateTime(2018, 3, 31, 11, 24, 03);
            var expectedValue = "Mar 31, 2018 11:24 AM";

            // Act
            var actualValue = dateTime.ToLongFormatedDateTime();

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void ToMonth_Always_ReturnCorrectValue()
        {
            // Arrange
            var dateTime = new DateTime(2018, 3, 31, 11, 24, 03);
            var expectedValue = "Mar";

            // Act
            var actualValue = dateTime.ToMonth();

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void FirstDayOfMonth_Always_ReturnCorrectValue()
        {
            // Arrange
            var dateTime = new DateTime(2018, 3, 31, 11, 24, 03);
            var expectedValue = new DateTime(2018, 3, 1);

            // Act
            var actualValue = dateTime.FirstDayOfMonth();

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void LastDayOfMonth_Always_ReturnCorrectValue()
        {
            // Arrange
            var dateTime = new DateTime(2018, 3, 31, 11, 24, 03);
            var expectedValue = new DateTime(2018, 3, 31);

            // Act
            var actualValue = dateTime.LastDayOfMonth();

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void FirstDayOfYear_Always_ReturnCorrectValue()
        {
            // Arrange
            var dateTime = new DateTime(2018, 3, 31, 11, 24, 03);
            var expectedValue = new DateTime(2018, 1, 1);

            // Act
            var actualValue = dateTime.FirstDayOfYear();

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void LastDayOfYear_Always_ReturnCorrectValue()
        {
            // Arrange
            var dateTime = new DateTime(2018, 3, 31, 11, 24, 03);
            var expectedValue = new DateTime(2018, 12, 31);

            // Act
            var actualValue = dateTime.LastDayOfYear();

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void GetEndOfDate_Always_ReturnCorrectValue()
        {
            // Arrange
            var dateTime = new DateTime(2018, 3, 31, 11, 24, 03);
            var expectedValue = new DateTime(2018, 3, 31, 23, 59, 59);

            // Act
            var actualValue = dateTime.GetEndOfDate();

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void GetStartOfDate_Always_ReturnCorrectValue()
        {
            // Arrange
            var dateTime = new DateTime(2018, 3, 31, 11, 24, 03);
            var expectedValue = new DateTime(2018, 3, 31);

            // Act
            var actualValue = dateTime.GetStartOfDate();

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void ToMonthYear_Always_ReturnCorrectValue()
        {
            // Arrange
            var dateTime = new DateTime(2018, 3, 31, 11, 24, 03);
            var expectedValue = "Mar 2018";

            // Act
            var actualValue = dateTime.ToMonthYear();

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void FirstDayOfWeek_Always_ReturnCorrectValue()
        {
            // Arrange
            var dateTime = new DateTime(2018, 3, 31, 11, 24, 03);
            var expectedValue = new DateTime(2018, 3, 25, 11, 24, 03);

            // Act
            var actualValue = dateTime.FirstDayOfWeek();

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void LastDayOfWeek_Always_ReturnCorrectValue()
        {
            // Arrange
            var dateTime = new DateTime(2018, 3, 31, 11, 24, 03);
            var expectedValue = new DateTime(2018, 3, 31, 11, 24, 03);

            // Act
            var actualValue = dateTime.LastDayOfWeek();

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }
    }
}
