using Fanex.Alpha.Before.Services;

namespace Fanex.Alpha.Before.UnitTests.Services
{
    public class IPServiceTests
    {
        [Theory]
        [InlineData("2001:0db8:85a3:0000:0000:8a2e:0370:7334", true, (ulong)2306291500588430132)]
        [InlineData("2001:0db8:85a3:0000:0000:8a2e:0370:7334", false, (ulong)3779667465975824672)]
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
            Assert.Equal(expectedIpNum, actualIpNum);
        }

        [Theory]
        [InlineData("1", (ulong)1)]
        [InlineData("::1", (ulong)1)]
        [InlineData("127.0.0.1", (ulong)2130706433)]
        public void ConvertIPV4ToNumber_Always_ReturnCorrectNum(
            string ip,
            ulong expectedIpNum)
        {
            // Arrange

            // Act
            var actualIpNum = IpServiceImpl.ConvertIPV4ToNumber(ip);

            // Assert
            Assert.Equal(expectedIpNum, actualIpNum);
        }
    }
}
