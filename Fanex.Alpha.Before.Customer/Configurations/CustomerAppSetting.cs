// -----------------------------------------------------------------------
// <copyright file="CustomerAppSetting.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Alpha.Before.Customer.Configurations
{
    using System.Configuration;
    using Fanex.Common;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class CustomerAppSetting
    {
        public static string AfterLoginAuthorizationPage
        {
            get
            {
                return Converter.ToString2(ConfigurationManager.AppSettings["AfterLoginPage"]);
            }
        }
    }
}
