// -----------------------------------------------------------------------
// <copyright file="ILimitedCountryAccessRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Alpha.Before.Common.Repositories.LimitedCountryAccessRepository
{
    /// <summary>
    /// Limited Country Access Repository Interface.
    /// </summary>
    public interface ILimitedCountryAccessRepository
    {
        string CountryCodeByIp();
    }
}
