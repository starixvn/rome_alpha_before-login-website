namespace Fanex.Alpha.Before.Common.Tests.Services
{
    using Common.Services;
    using NUnit.Framework;

    [TestFixture]
    public class IPServiceTests
    {
        [TestCase("2001:0db8:85a3:0000:0000:8a2e:0370:7334", true, (ulong)2306291500588430132)]
        [TestCase("2001:0db8:85a3:0000:0000:8a2e:0370:7334", false, (ulong)3779667465975824672)]
        public void ConvertIPV6ToNumber_Always_ReturnCorrectNum(
            string ip,
            bool littleEndian,
            ulong expectedIpNum)
        {
            // Arrange
            IpServiceImpl.SetIsLittleEndian(littleEndian);

            // Act
            var actualIpNum = IpServiceImpl.ConvertIPV6ToNumber(ip);

            // Assert
            Assert.AreEqual(expectedIpNum, actualIpNum);
        }

        [TestCase("1", (ulong)1)]
        [TestCase("::1", (ulong)1)]
        [TestCase("127.0.0.1", (ulong)2130706433)]
        public void ConvertIPV4ToNumber_Always_ReturnCorrectNum(
            string ip,
            ulong expectedIpNum)
        {
            // Arrange

            // Act
            var actualIpNum = IpServiceImpl.ConvertIPV4ToNumber(ip);

            // Assert
            Assert.AreEqual(expectedIpNum, actualIpNum);
        }
    }
}
