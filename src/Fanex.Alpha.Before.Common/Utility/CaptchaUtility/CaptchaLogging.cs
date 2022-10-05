// -----------------------------------------------------------------------
// <copyright file="CaptchaLogging.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Alpha.Before.Common.Utility.CaptchaUtility
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class CaptchaLogging
    {
        private const int _MAX_LIST_SIZE = 10000;
        private static List<CaptchaInfo> captchaList = new List<CaptchaInfo>();
        private static object lockObj = new object();

        /// <summary>
        /// Log method.
        /// </summary>
        /// <param name="ip">The customer's IP.</param>
        /// <param name="username">The Username.</param>
        /// <param name="type">The captcha type.</param>
        public static void Log(string ip, string username, CaptchaType type)
        {
            // only logging in question
            if (type == CaptchaType.Question)
            {
                CaptchaInfo info = Find(ip, username);
                if (info == null)
                {
                    info = new CaptchaInfo
                    {
                        Ip = ip,
                        Username = username,
                        Type = type
                    };
                    lock (lockObj)
                    {
                        captchaList.Add(info); // add last
                        while (captchaList.Count > _MAX_LIST_SIZE)
                        {
                            captchaList.RemoveAt(0); // remove first
                        }
                    }
                }
            }
        }

        public static void RemoveLog(string ip, string username)
        {
            CaptchaInfo info = Find(ip, username);
            if (info != null)
            {
                lock (lockObj)
                {
                    captchaList.Remove(info);
                }
            }
        }

        /// <summary>
        /// Finding captcha.
        /// </summary>
        /// <param name="ip">The customer's IP.</param>
        /// <param name="username">The username.</param>
        /// <returns>Return result.</returns>
        public static CaptchaInfo Find(string ip, string username)
        {
            lock (lockObj)
            {
                foreach (var info in captchaList)
                {
                    if (info.Ip == ip || string.Equals(info.Username, username, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return info;
                    }
                }
            }

            return null;
        }
    }
}
