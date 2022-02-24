// -----------------------------------------------------------------------
// <copyright file="SignInResult.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Fanex.Alpha.Before.Common.Configurations;
using Fanex.Alpha.Before.Common.Helpers;
using Fanex.Alpha.Before.Common.Models;

namespace Fanex.Alpha.Before.Customer.Models
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SignInResult
    {
        public int ErrCode
        {
            get;
            set;
        }

        public string ErrMsg
        {
            get;
            set;
        }

        public int Language
        {
            get;
            set;
        }

        public string Ip
        {
            get;
            set;
        }

        public string UTMS
        {
            get;
            set;
        }

        public string Domain
        {
            get;
            set;
        }

        public string Captcha
        {
            get;
            set;
        }

        public bool IsChangedPassword
        {
            get;
            set;
        }

        public string ChangedPasswordTime
        {
            get;
            set;
        }

        public string AfterLoginUrl
        {
            get;
            set;
        }

        public string LoginTemplate
        {
            get;
            set;
        }

        public string Username
        {
            get;
            set;
        }

        public GoogleReCaptcha GoogleReCaptcha
        {
            get;
            set;
        }

        public bool IsCaptchaVerified { get; set; }

        public string AssetAppPaths { get; } = AppSetting.AssetAppPaths;

        public bool IsOfferDarkMode { get; } = DarkModeHelper.IsOfferDarkMode;
    }
}
