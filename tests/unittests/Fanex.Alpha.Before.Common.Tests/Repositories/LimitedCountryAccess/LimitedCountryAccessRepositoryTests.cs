namespace Fanex.Alpha.Before.Common.Tests.Repositories.LimitedCountryAccess
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using Common.Criteria.LimitedCountryAccess;
    using Common.Models;
    using Common.Repositories;
    using Common.Repositories.LimitedCountryAccess;
    using Data;
    using Database.LimitedCountryAccess;
    using KellermanSoftware.CompareNetObjects;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class LimitedCountryAccessRepositoryTests
    {
        private LimitedCountryAccessRepository limitedCountryAccessRepository;
        private IObjectDb objectDb;
        private CompareLogic compareLogic;

        [SetUp]
        public void Init()
        {
            limitedCountryAccessRepository = new LimitedCountryAccessRepository();

            objectDb = Substitute.For<IObjectDb>();
            ObjectDbFactory.CreateInstance = (settingKey) => objectDb;
            compareLogic = new CompareLogic();
        }

        [Test]
        public void GetIPRangesByCountry_NoData_ReturnEmpty()
        {
            // Arrange
            objectDb.ExecuteReader(Arg.Any<Hashtable>()).Returns((IDataReader)null);

            // Act
            var actualData = limitedCountryAccessRepository.GetIPRangesByCountry(new GetIpRangesByCountryCriteria(false, string.Empty));

            // Assert
            Assert.IsEmpty(actualData);
        }

        [Test]
        public void GetIPRangesByCountry_HasData_ReturnCorrectData()
        {
            // Arrange
            var reportData = LimitedCountryAccessDataBase.IpRangesByCountry.CreateDataReader();
            objectDb.ExecuteReader(Arg.Any<Hashtable>()).Returns(reportData);
            var expectedData = new List<IpRange>
            {
                new IpRange(1, 3, "HKK")
            };

            // Act
            var actualData = limitedCountryAccessRepository.GetIPRangesByCountry(new GetIpRangesByCountryCriteria(false, string.Empty));

            // Assert
            Assert.IsTrue(compareLogic.Compare(expectedData, actualData).AreEqual);
        }

        [Test]
        public void GetsAllowedAccessIPs_NoData_ReturnEmpty()
        {
            // Arrange
            objectDb.ExecuteReader(Arg.Any<Hashtable>()).Returns((IDataReader)null);

            // Act
            var actualData = limitedCountryAccessRepository.GetsAllowedAccessIPs(new GetAllowedAccessIPsCriteria());

            // Assert
            Assert.IsEmpty(actualData);
        }

        [Test]
        public void GetsAllowedAccessIPs_HasData_ReturnCorrectData()
        {
            // Arrange
            var reportData = LimitedCountryAccessDataBase.AllowedAccessIPs.CreateDataReader();
            objectDb.ExecuteReader(Arg.Any<Hashtable>()).Returns(reportData);
            var expectedData = new List<string> { "127.0.0.1" };

            // Act
            var actualData = limitedCountryAccessRepository.GetsAllowedAccessIPs(new GetAllowedAccessIPsCriteria());

            // Assert
            Assert.AreEqual(expectedData, actualData);
        }
    }
}
