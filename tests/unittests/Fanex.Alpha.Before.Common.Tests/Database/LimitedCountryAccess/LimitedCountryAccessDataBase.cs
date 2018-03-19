namespace Fanex.Alpha.Before.Common.Tests.Database.LimitedCountryAccess
{
    using System.Data;
    using Helpers;

    public static class LimitedCountryAccessDataBase
    {
        private static DataSet ipRangesByCountry = FileHelper.LoadDataSet("\\Database\\LimitedCountryAccess\\IPRangesByCountry.xml");
        private static DataSet allowedAccessIPs = FileHelper.LoadDataSet("\\Database\\LimitedCountryAccess\\AllowedAccessIPs.xml");

        public static DataSet IpRangesByCountry => ipRangesByCountry;

        public static DataSet AllowedAccessIPs => allowedAccessIPs;
    }
}
