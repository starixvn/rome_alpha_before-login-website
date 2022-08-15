// -----------------------------------------------------------------------
// <copyright file="UrlHelper.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Alpha.Before.Common.Helpers
{
    using System.Linq;
    using System.Web;
    using Fanex.Alpha.Before.Common.Configurations.Domain;
    using Fanex.Alpha.Before.Common.Models;
    using Fanex.Alpha.Before.Customer.Configurations;
    using System;

    /// <summary>
    /// URL helper.
    /// </summary>
    public static class UrlHelper
    {
        internal const string HttpSlashSlash = "http://";
        internal const string HttpSlashSlashS = "https://";
        internal const char UrlSlash = '/';

        public static SiteGroupModel SiteGroupSetting
        {
            get
            {
                var request = HttpContext.Current.Request;
                var hostName = request.Headers["Host"];
                var appPath = request.Path.TrimEnd('/');
                var protocol = request.IsSecureConnection ? HttpSlashSlashS : HttpSlashSlash;
                var hasAppPath = appPath.Length > 1;
                var slash = hasAppPath ? "/" : string.Empty;
                var requestURL = $"{protocol}{hostName}{appPath}{slash}";

                var domainGroupSection = DomainGroupSection.Instance.DomainGroup;
                var baseUrl = GetBaseUrl();
                const string defaultGroup = "Others";
                var siteGroup = new SiteGroupModel
                {
                    SiteGroup = defaultGroup,
                    BeforeUrl = baseUrl,
                    AfterUrl = baseUrl,
                    GoogleReCaptcha = new GoogleReCaptcha()
                };

                if (domainGroupSection == null || domainGroupSection.Count == 0)
                {
                    return siteGroup;
                }

                var domainGroupActive = from m in domainGroupSection.Cast<DomainGroupElement>().Where(m => m.Active) select m;

                if (!domainGroupActive.Any())
                {
                    return siteGroup;
                }

                foreach (DomainGroupElement domainGroupElement in domainGroupActive)
                {
                    if (domainGroupElement.Domains == null || domainGroupElement.Domains.Count == 0)
                    {
                        return siteGroup;
                    }

                    var domains = from d in domainGroupElement.Domains.Cast<DomainElement>().Where(d => d.Active) select d;

                    if (!domains.Any())
                    {
                        return siteGroup;
                    }

                    foreach (var domainSetting in domains)
                    {
                        if (requestURL.IndexOf(domainSetting.BeforeUrl, StringComparison.OrdinalIgnoreCase) == -1)
                        {
                            continue;
                        }

                        siteGroup.SiteGroup = domainGroupElement.Name;
                        siteGroup.BeforeUrl = domainSetting.BeforeUrl;
                        siteGroup.AfterUrl = domainSetting.AfterUrl;
                        siteGroup.GoogleReCaptcha = new GoogleReCaptcha(domainGroupElement.SiteKey, domainGroupElement.SecretKey, domainGroupElement.Proxy);

                        if (AlphaCommonSection.Instance != null &&
                           !string.IsNullOrEmpty(AlphaCommonSection.Instance.OfficeIPs) &&
                            AlphaCommonSection.Instance.OfficeIPs.Split(',').Contains(IPHelper.ClientIP()))
                        {
                            siteGroup.SiteGroup = defaultGroup;
                        }

                        return siteGroup;
                    }
                }

                return siteGroup;
            }
        }

        /// <summary>
        /// Get base url of current website.
        /// </summary>
        /// <returns>Return URL.</returns>
        private static string GetBaseUrl()
        {
            var hostName = HttpContext.Current.Request.Headers["Host"];
            var appPath = HttpContext.Current.Request.Path.TrimEnd('/');
            var url = string.Format(
                "{0}{1}{2}",
                HttpContext.Current.Request.IsSecureConnection ? HttpSlashSlashS : HttpSlashSlash,
                hostName,
                appPath);

            if (appPath.Length > 1)
            {
                url += UrlSlash;
            }

            return url;
        }
    }
}
