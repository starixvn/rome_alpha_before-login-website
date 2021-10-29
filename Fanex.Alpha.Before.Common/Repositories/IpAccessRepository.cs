namespace Fanex.Alpha.Before.Common.Repositories
{
    using Fanex.Alpha.Before.Common.Criteria;
    using Fanex.Common;
    using Models;

    public interface IIpAccessRepository
    {
        IPInformationModel GetIPInformation(GetIPInformationCriteria criteria);
    }

    public class IpAccessRepository : BaseRepository, IIpAccessRepository
    {
        public IPInformationModel GetIPInformation(GetIPInformationCriteria criteria)
        {
            var result = new IPInformationModel();

            if (criteria.IsValid())
            {
                using (var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
                {
                    using (var reader = objectDb.ExecuteReader(criteria.GetSpParams()))
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                result = new IPInformationModel
                                {
                                    CountryShort = Converter.ToString2(reader["CountryShort"]),
                                    CountryLong = Converter.ToString2(reader["CountryLong"]),
                                    IpRegion = Converter.ToString2(reader["IpRegion"]),
                                    IpCity = Converter.ToString2(reader["IpCity"])
                                };
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}
