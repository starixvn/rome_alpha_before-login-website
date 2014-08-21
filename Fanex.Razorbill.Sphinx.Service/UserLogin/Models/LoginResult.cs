// -----------------------------------------------------------------------
// <copyright file="LoginResult.cs" company="Nexcel Solution">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Razorbill.Sphinx.Service.UserLogin.Models
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LoginResult
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
