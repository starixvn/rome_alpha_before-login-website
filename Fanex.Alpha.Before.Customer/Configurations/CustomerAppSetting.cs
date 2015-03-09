// -----------------------------------------------------------------------
// <copyright file="CustomerAppSetting.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Alpha.Before.Customer.Configurations
{
    using System.Configuration;
    using Fanex.Common;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CustomerAppSetting
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
