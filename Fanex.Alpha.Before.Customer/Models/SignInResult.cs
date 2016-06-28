// -----------------------------------------------------------------------
// <copyright file="SignInResult.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

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
    }
}
