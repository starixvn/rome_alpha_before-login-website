// -----------------------------------------------------------------------
// <copyright file="LimitedCountryAccessRepository.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Alpha.Before.Common.Repositories.LimitedCountryAccessRepository
{
    using System.Collections;
    using Fanex.Common;
    using Fanex.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LimitedCountryAccessRepository : BaseRepository, ILimitedCountryAccessRepository
    {
        /// <summary>
        /// Get country code by customer's IP.
        /// </summary>
        /// <returns>Return country code.</returns>
        public string CountryCodeByIp()
        {
            using (ObjectDb objectDb = new ObjectDb("Age_CheckValidCountry"))
            {
                objectDb.AssignParamValues(new Hashtable()
                {
                    { "ip", Helpers.IPHelper.ClientIP() },
                    { "countrycode", string.Empty }
                });

                objectDb.ExecuteNonQuery();

                return Converter.ToString2(objectDb.GetOuputParams()["countrycode"]);
            }
        }
    }
}
