// -----------------------------------------------------------------------
// <copyright file="ILimitedCountryAccessService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Alpha.Before.Common.Configurations.LimitedCountryAccessService
{
    /// <summary>
    /// Limited countries access service.
    /// </summary>
    public interface ILimitedCountryAccessService
    {
        bool LimitedAccess();
    }
}
