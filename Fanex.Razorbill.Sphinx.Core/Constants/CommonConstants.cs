// -----------------------------------------------------------------------
// <copyright file="CommonConstants.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Razorbill.Sphinx.Core.Constants
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public class CommonConstants
    {
        public const string CAPTCHA_2 = "Fanex.Razorbill.Sphinx.Core.Constants.captcha2";
        public const string CAPTCHA_3 = "Fanex.Razorbill.Sphinx.Core.Constants.captcha3";
        public const string CAPTCHA_CODE = "Fanex.Razorbill.Sphinx.Core.Constants.captchacode";
        public const string USERNAME = "Fanex.Razorbill.Sphinx.Core.Constants.username";
        public const string LANGUAGE = "Fanex.Razorbill.Sphinx.Core.Constants.language";
        public const string LOGINFAILEDCOUNT = "Fanex.Razorbill.Sphinx.Core.Constants.loginfailedcount.{0}";
        public const string LOGINFAILEDTIME = "Fanex.Razorbill.Sphinx.Core.Constants.loginfailedtime.{0}";

        public const string DOMAINS_EXCLUDE_DOWNTIME = "DomainsExcludeDowntime";
        public const string BASE_DOMAIN = "BaseDomain";
        public const string DEFAULT_LOGIN_TEMPLATE = "DefaultLoginTemplate";
        public const string EVENT_LOGIN_TEMPLATE = "EventLoginTemplate";
        public const string SITE_IBC_GROUP = "IBCGroup";
        public const string SITE_SPECIAL_GROUP = "SpecialGroup";
        public const string AFTER_LOGIN_PAGE = "AfterLoginPage";
    }
}
