namespace Fanex.Alpha.Before.Common.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Common.Criteria.LimitedCountryAccess;
    using Common.Models;
    using Common.Repositories.LimitedCountryAccess;
    using Common.Services;
    using Common.Services.LimitedCountryAccessService;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class LimitedCountryAccessServiceTests
    {
        private const string AllowedAccessIPsCachedKey = "Fanex.Alpha.Before.Common.AllowedAccessIPsCachedKey";
        private const string RestrictedCountriesCachedKey = "Fanex.Alpha.Before.Common.RestrictedCountriesCachedKey";

        private LimitedCountryAccessService limitedCountryAccessService;
        private ILimitedCountryAccessRepository ipRepository;
        private IPService ipService;
        private ICacheService cacheService;
        private ILimitedCountriesConfigService limitedCountriesConfigService;
        private  Action<Exception> logInfoAction;

        private IList<LimitedCountriesConfigModel> limitedCountriesConfig;

        [SetUp]
        public void Init()
        {
            IpServiceImpl.SetIsLittleEndian(true);

            ipRepository = Substitute.For<ILimitedCountryAccessRepository>();
            ipService = Substitute.For<IPService>();
            cacheService = Substitute.For<ICacheService>();
            limitedCountriesConfigService = Substitute.For<ILimitedCountriesConfigService>();
            logInfoAction = Substitute.For<Action<Exception>>();

            limitedCountryAccessService = new LimitedCountryAccessService(false, ipRepository, ipService, cacheService, limitedCountriesConfigService, logInfoAction);

            limitedCountriesConfig = new List<LimitedCountriesConfigModel>
            {
                new LimitedCountriesConfigModel("HKK", true, "127.0.0.0")
            };
        }

        [Test]
        public void IsLimitedAccess_AccessWithIPv6_WriteInfo()
        {
            // Arrange
            ipService
               .GetClientIp()
               .Returns("2001:0db8:85a3:0000:0000:8a2e:0370:7334");
            var expectedMessage = "Customer Access via IPV6: 2001:0db8:85a3:0000:0000:8a2e:0370:7334";

            // Act
            limitedCountryAccessService.IsLimitedAccess();

            // Assert
            logInfoAction.Received().Invoke(Arg.Is<Exception>(ex => ex.Message == expectedMessage));
        }

        [Test]
        public void IsLimitedAccess_AccessWithIPv4_NotWriteInfo()
        {
            // Arrange
            ipService
               .GetClientIp()
               .Returns("127.0.0.1");

            // Act
            limitedCountryAccessService.IsLimitedAccess();

            // Assert
            logInfoAction.DidNotReceive();
        }

        [Test]
        public void GetsAllowedAccessIPs_Always_CacheDataWithCorrectKey()
        {
            // Act
            limitedCountryAccessService.GetsAllowedAccessIPs();

            // Assert
            cacheService.Received().GetOrSet(
                AllowedAccessIPsCachedKey,
                Arg.Any<Func<IList<string>>>(),
                Arg.Any<TimeSpan>());
        }

        [Test]
        public void GetsAllowedAccessIPs_Always_CacheDataFor10Minutes()
        {
            // Act
            limitedCountryAccessService.GetsAllowedAccessIPs();

            // Assert
            cacheService.Received().GetOrSet(
                AllowedAccessIPsCachedKey,
                Arg.Any<Func<IList<string>>>(),
                Arg.Is<TimeSpan>(timeSpan => timeSpan.TotalMinutes == 10));
        }

        [Test]
        public void GetsAllowedAccessIPs_Always_GetDataWhenCacheNotHaveData()
        {
            // Arrange
            var data = new List<string> { "sampleIP" };

            ipRepository.GetsAllowedAccessIPs(Arg.Any<GetAllowedAccessIPsCriteria>())
                .Returns(data);

            // Act
            limitedCountryAccessService.GetsAllowedAccessIPs();

            // Assert
            cacheService.Received().GetOrSet(
                AllowedAccessIPsCachedKey,
                Arg.Is<Func<IList<string>>>(f => f() == data),
                Arg.Any<TimeSpan>());
        }

        [Test]
        public void GetsAllowedAccessIPs_Always_ReturnCorrectData()
        {
            // Arrange
            var data = new List<string> { "sampleIP" };

            cacheService.GetOrSet(Arg.Any<string>(), Arg.Any<Func<IList<string>>>(), Arg.Any<TimeSpan>())
               .Returns(data);

            // Act
            var actualData = limitedCountryAccessService.GetsAllowedAccessIPs();

            // Assert
            Assert.AreEqual(data, actualData);
        }

        [TestCase(false, "Fanex.Alpha.Before.Common.RestrictedIPRangesCachedKey_IPV4_")]
        [TestCase(true, "Fanex.Alpha.Before.Common.RestrictedIPRangesCachedKey_IPV6_")]
        public void GetIPRangesByRestrictedCountry_Always_CallGetOrSetToCacheData(
            bool isIpV6,
            string expectedCacheKey)
        {
            // Act
            limitedCountryAccessService.GetIPRangesByRestrictedCountry(Enumerable.Empty<LimitedCountriesConfigModel>(), isIpV6);

            // Assert
            cacheService.Received().GetOrSet(
                expectedCacheKey,
                Arg.Any<Func<IList<IpRange>>>(),
                Arg.Any<TimeSpan>());
        }

        [TestCase(false, "Fanex.Alpha.Before.Common.RestrictedIPRangesCachedKey_IPV4_")]
        [TestCase(true, "Fanex.Alpha.Before.Common.RestrictedIPRangesCachedKey_IPV6_")]
        public void GetIPRangesByRestrictedCountry_Always_CacheDataFor30Days(bool isIpV6, string expectedCacheKey)
        {
            // Act
            limitedCountryAccessService.GetIPRangesByRestrictedCountry(Enumerable.Empty<LimitedCountriesConfigModel>(), isIpV6);

            // Assert
            cacheService.Received().GetOrSet(
                expectedCacheKey,
                Arg.Any<Func<IList<IpRange>>>(),
                Arg.Is<TimeSpan>(timeSpan => timeSpan.TotalDays == 30));
        }

        [TestCase(false, "Fanex.Alpha.Before.Common.RestrictedIPRangesCachedKey_IPV4_MNL,UK")]
        [TestCase(true, "Fanex.Alpha.Before.Common.RestrictedIPRangesCachedKey_IPV6_MNL,UK")]
        public void GetIPRangesByRestrictedCountry_Always_GetDataWhenCacheNotHaveData(bool isIpV6, string expectedCacheKey)
        {
            // Arrange
            var data = new List<IpRange>();
            var limitCountry = new List<LimitedCountriesConfigModel>
            {
                new LimitedCountriesConfigModel("MNL", false, null),
                new LimitedCountriesConfigModel("UK", false, null)
            };

            ipRepository.GetIPRangesByCountry(
                Arg.Is<GetIpRangesByCountryCriteria>(c => c.CountryCodes == "MNL,UK"))
                .Returns(data);

            // Act
            limitedCountryAccessService.GetIPRangesByRestrictedCountry(limitCountry, isIpV6);

            // Assert
            cacheService.Received().GetOrSet(
                expectedCacheKey,
                Arg.Is<Func<IList<IpRange>>>(f => f() == data),
                Arg.Any<TimeSpan>());
        }

        [TestCase(false, "Fanex.Alpha.Before.Common.RestrictedIPRangesCachedKey_IPV4")]
        [TestCase(true, "Fanex.Alpha.Before.Common.RestrictedIPRangesCachedKey_IPV6")]
        public void GetIPRangesByRestrictedCountry_Always_ReturnCorrectData(bool isIpV6, string expectedCacheKey)
        {
            // Arrange
            var data = new List<IpRange> { };
            var limitCountry = new List<LimitedCountriesConfigModel> {
                new LimitedCountriesConfigModel("HKK", false, null)
            };

            cacheService.GetOrSet(expectedCacheKey, Arg.Any<Func<IList<IpRange>>>(), Arg.Any<TimeSpan>())
                .Returns(data);

            // Act
            var actualData = limitedCountryAccessService.GetIPRangesByRestrictedCountry(limitCountry, isIpV6);

            // Assert
            Assert.AreEqual(data, actualData);
        }

        [Test]
        public void LimitedAccess_NotHaveActiveRestrictedCountry_ReturnFalse()
        {
            // Arrange
            limitedCountriesConfigService.GetActiveRestrictedCountriesConfig()
                .Returns(Enumerable.Empty<LimitedCountriesConfigModel>());

            // Act
            var actualLimitedAccess = limitedCountryAccessService.IsLimitedAccess();

            // Assert
            Assert.IsFalse(actualLimitedAccess);
        }

        [Test]
        public void LimitedAccess_ClientIPInTestedConfig_ReturnTrue()
        {
            // Arrange
            limitedCountriesConfigService.GetActiveRestrictedCountriesConfig()
                .Returns(limitedCountriesConfig);
            ipService.GetClientIp().Returns("127.0.0.0");

            // Act
            var actualLimitedAccess = limitedCountryAccessService.IsLimitedAccess();

            // Assert
            Assert.IsTrue(actualLimitedAccess);
        }

        [Test]
        public void LimitedAccess_V6IPNotInRestrictedRange_ReturnFalse()
        {
            // Arrange
            limitedCountriesConfigService
                .GetActiveRestrictedCountriesConfig()
                .Returns(limitedCountriesConfig);

            ipService
                .GetClientIp()
                .Returns("2001:0db8:85a3:0000:0000:8a2e:0370:7334");

            cacheService
                .GetOrSet(
                "Fanex.Alpha.Before.Common.RestrictedIPRangesCachedKey_IPV6_HKK",
                Arg.Any<Func<IList<IpRange>>>(),
                Arg.Any<TimeSpan>())
                .Returns(
                new List<IpRange> {
                    new IpRange(2306291500588430130, 2306291500588430131, "HKK")
                });

            // Act
            var actualLimitedAccess = limitedCountryAccessService.IsLimitedAccess();

            // Assert
            Assert.IsFalse(actualLimitedAccess);
        }

        [Test]
        public void LimitedAccess_V6IPInAllowedAccessIPs_ReturnFalse()
        {
            // Arrange
            limitedCountriesConfigService
                .GetActiveRestrictedCountriesConfig()
                .Returns(limitedCountriesConfig);

            ipService
                .GetClientIp()
                .Returns("2001:0db8:85a3:0000:0000:8a2e:0370:7334");

            cacheService
                .GetOrSet(
                "Fanex.Alpha.Before.Common.RestrictedIPRangesCachedKey_IPV6",
                Arg.Any<Func<IList<IpRange>>>(),
                Arg.Any<TimeSpan>())
                .Returns(new List<IpRange>
                {
                    new IpRange(2306291500588430132, 2306291500588430132, "HKK")
                });

            cacheService.GetOrSet(
                Arg.Any<string>(),
                Arg.Any<Func<IList<string>>>(),
                Arg.Any<TimeSpan>())
               .Returns(new List<string> { "2001:0db8:85a3:0000:0000:8a2e:0370:7334" });

            // Act
            var actualLimitedAccess = limitedCountryAccessService.IsLimitedAccess();

            // Assert
            Assert.IsFalse(actualLimitedAccess);
        }

        [Test]
        public void LimitedAccess_V6IPNotInAllowedAccessIPs_ReturnTrue()
        {
            // Arrange
            limitedCountriesConfigService
                .GetActiveRestrictedCountriesConfig()
                .Returns(limitedCountriesConfig);

            ipService
                .GetClientIp()
                .Returns("2001:0db8:85a3:0000:0000:8a2e:0370:7334");

            cacheService.GetOrSet(
                "Fanex.Alpha.Before.Common.RestrictedIPRangesCachedKey_IPV6_HKK",
                Arg.Any<Func<IList<IpRange>>>(),
                Arg.Any<TimeSpan>())
                .Returns(new List<IpRange>
                {
                    new IpRange(2306291500588430132, 2306291500588430132, "HKK")
                });

            cacheService.GetOrSet(
                "Fanex.Alpha.Before.Common.AllowedAccessIPsCachedKey",
                Arg.Any<Func<IList<string>>>(),
                Arg.Any<TimeSpan>())
               .Returns(new List<string> { "2001:0db8:85a3:0000:0000:8a2e:0370:7335" });

            // Act
            var actualLimitedAccess = limitedCountryAccessService.IsLimitedAccess();

            // Assert
            Assert.IsTrue(actualLimitedAccess);
        }

        [Test]
        public void LimitedAccess_V4IPNotInAllowedAccessIPs_ReturnTrue()
        {
            // Arrange
            limitedCountriesConfigService.GetActiveRestrictedCountriesConfig()
                .Returns(limitedCountriesConfig);
            ipService.GetClientIp().Returns("127.0.0.1");
            cacheService.GetOrSet(
                "Fanex.Alpha.Before.Common.RestrictedIPRangesCachedKey_IPV4_HKK",
                Arg.Any<Func<IList<IpRange>>>(),
                Arg.Any<TimeSpan>())
                .Returns(new List<IpRange>
                {
                    new IpRange(2130706433, 2130706433, "HKK")
                });

            cacheService.GetOrSet(
                Arg.Any<string>(),
                Arg.Any<Func<IList<string>>>(),
                Arg.Any<TimeSpan>())
               .Returns(new List<string> { "127.0.0.2" });

            // Act
            var actualLimitedAccess = limitedCountryAccessService.IsLimitedAccess();

            // Assert
            Assert.IsTrue(actualLimitedAccess);
        }
    }
}
