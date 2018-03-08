namespace Fanex.Alpha.Before.Common.Repositories.LimitedCountryAccessRepository
{
    using System.Collections.Generic;
    using Criteria.LimitedCountryAccess;
    using Data;
    using Fanex.Common;
    using Models;

    public class LimitedCountryAccessRepository : BaseRepository, ILimitedCountryAccessRepository
    {
        public IList<IpRange> GetIPRangesByCountry(GetIpRangesByCountryCriteria criteria)
        {
            var result = new List<IpRange>();

            using (ObjectDb objectDb = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var reader = objectDb.ExecuteReader(criteria.GetSpParams()))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var ipRange = new IpRange(
                                ipFrom: Converter.To<long>(reader["IPFrom"]),
                                ipTo: Converter.To<long>(reader["IPTo"]),
                                countryCode: Converter.ToString2(reader["CountryShort"]));

                            result.Add(ipRange);
                        }
                    }
                }
            }

            return result;
        }

        public IList<string> GetsAllowedAccessIPs(GetAllowedAccessIPsCriteria criteria)
        {
            var result = new List<string>();

            using (ObjectDb objectDb = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var reader = objectDb.ExecuteReader(criteria.GetSpParams()))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            result.Add(Converter.ToString2(reader["IP"]));
                        }
                    }
                }
            }

            return result;
        }
    }
}
