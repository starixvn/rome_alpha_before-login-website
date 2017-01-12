// -----------------------------------------------------------------------
// <copyright file="LimitedCountryAccessService.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Alpha.Before.Common.Configurations.LimitedCountryAccessService
{
    using System;
    using System.Linq;
    using Fanex.Alpha.Before.Common.Configurations.LimtedCountryAccess;
    using Fanex.Alpha.Before.Common.Repositories.LimitedCountryAccessRepository;
    using Infrastructure.Extensions;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LimitedCountryAccessService : ILimitedCountryAccessService
    {
        public LimitedCountryAccessService()
        {
        }

        /// <summary>
        /// Checking Limited country access.
        /// </summary>
        /// <returns>Return blocking status.</returns>
        public bool LimitedAccess()
        {
            try
            {
                var limitedCountryAccessSection = LimtedCountryAccessSection.Instance.LimitedCountries;
                if (limitedCountryAccessSection == null ||
                    limitedCountryAccessSection.Count == 0)
                {
                    return false;
                }

                var limitedCountriesActive = from m in limitedCountryAccessSection.Cast<LimitedCountriesElement>().Where(m => m.Active) select m;

                if (!limitedCountriesActive.Any())
                {
                    return false;
                }

                var customerCountryCode = from m in limitedCountriesActive
                                          where ((m.Name.ToUpper().Equals(
                                          new LimitedCountryAccessRepository().CountryCodeByIp().ToUpper()) &&
                                          DateTime.Now >= m.AffectedDate) || //// real blocking country ip
                                                (m.TestingIp.Split(',').Contains(Helpers.IPHelper.ClientIP()) && //// testing ip
                                                    m.IsTesting == true && DateTime.Now >= m.AffectedDate))
                                          select m;

                if (customerCountryCode.Any())
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogExtension.ExceptionHandler(ex);
            }

            return false;
        }
    }
}