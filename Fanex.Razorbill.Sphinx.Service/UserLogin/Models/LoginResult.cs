// -----------------------------------------------------------------------
// <copyright file="LoginResult.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Razorbill.Sphinx.Service.UserLogin.Models
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public class LoginResult
    {
        public int ErrCode { get; set; }
        public string ErrMsg { get; set; }
        public int Language { get; set; }
        public string Ip { get; set; }
        public string UTMS { get; set; }
        public string Domain { get; set; }
        public string Captcha { get; set; }
        public bool IsChangedPassword { get; set; }
        public string ChangedPasswordTime { get; set; }
        public string AfterLoginUrl { get; set; }
        public string LoginTemplate { get; set; }
        public string Username { get; set; }
    }
}
