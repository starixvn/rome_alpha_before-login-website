// -----------------------------------------------------------------------
// <copyright file="VerifySignInResult.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Alpha.Before.Customer.Models
{
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class VerifySignInResult
    {
        public int CustId 
        { 
            get; 
            set; 
        }

        public int SubAccId 
        { 
            get; 
            set; 
        }

        public DateTime LoginTime 
        { 
            get;
            set; 
        }

        public DateTime LastTimeToChangePassword 
        { 
            get; 
            set; 
        }

        public string Nickname 
        { 
            get; 
            set; 
        }

        public string Username
        {
            get;
            set;
        }

        public bool IsNickname 
        { 
            get; 
            set; 
        }

        public int ErrorCode 
        { 
            get;
            set; 
        }

        public string ErrorMessage 
        { 
            get;
            set; 
        }
    }
}
