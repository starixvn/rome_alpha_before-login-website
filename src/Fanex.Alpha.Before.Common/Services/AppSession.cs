// -----------------------------------------------------------------------
// <copyright file="AppSession.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Alpha.Before.Common.Configurations
{
    using Fanex.Alpha.Before.Common.Constants;
    using Fanex.Alpha.Before.Infrastructure.Extensions;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class AppSession
    {
        public static string Language
        {
            get
            {
                return SessionExtension.Get<string>(SessionConstants.LANGUAGE);
            }

            set
            {
                SessionExtension.Set<string>(SessionConstants.LANGUAGE, value);
            }
        }

        public static string LangKey
        {
            get
            {
                switch (AppSession.Language)
                {
                    case "en-US": return "E";
                    case "ja-JP": return "J";
                    case "zh-TW": return "CT"; // Traditional
                    case "zh-CN": return "CS"; // Simplified
                    case "th-TH": return "TH";
                    case "ko-KR": return "KO";
                    case "vi-VN": return "VI";
                    default: return "E";
                }
            }
        }

        public static string EncryptCode
        {
            get
            {
                return SessionExtension.Get<string>(SessionConstants.ENCRYPTCODE);
            }

            set
            {
                SessionExtension.Set<string>(SessionConstants.ENCRYPTCODE, value);
            }
        }

        public static string CaptchaCode
        {
            get
            {
                return SessionExtension.Get<string>(SessionConstants.CAPTCHACODE);
            }

            set
            {
                SessionExtension.Set<string>(SessionConstants.CAPTCHACODE, value);
            }
        }

        public static string CookieCode
        {
            get
            {
                return SessionExtension.Get<string>(SessionConstants.COOKIECODE);
            }

            set
            {
                SessionExtension.Set<string>(SessionConstants.COOKIECODE, value);
            }
        }

        public static string Username
        {
            get
            {
                return SessionExtension.Get<string>(SessionConstants.USERNAME);
            }

            set
            {
                SessionExtension.Set<string>(SessionConstants.USERNAME, value);
            }
        }

        public static string DarkModeName
        {
            get
            {
                return SessionExtension.Get<string>(SessionConstants.DARKMODENAME) ?? string.Empty;
            }

            set
            {
                SessionExtension.Set(SessionConstants.DARKMODENAME, value);
            }
        }

        #region Robot Captcha

        public static int CustId
        {
            get
            {
                return SessionExtension.Get<int>(SessionConstants.CUSTID);
            }

            set
            {
                SessionExtension.Set(SessionConstants.CUSTID, value);
            }
        }

        public static int SubAccId
        {
            get
            {
                return SessionExtension.Get<int>(SessionConstants.SUBACCID);
            }

            set
            {
                SessionExtension.Set(SessionConstants.SUBACCID, value);
            }
        }

        public static string SubAccName
        {
            get
            {
                return SessionExtension.Get<string>(SessionConstants.SUBACCNAME);
            }

            set
            {
                SessionExtension.Set(SessionConstants.SUBACCNAME, value);
            }
        }

        public static bool IsNickname
        {
            get
            {
                return SessionExtension.Get<bool>(SessionConstants.ISNICKNAME);
            }

            set
            {
                SessionExtension.Set(SessionConstants.ISNICKNAME, value);
            }
        }

        public static string Nickname
        {
            get
            {
                return SessionExtension.Get<string>(SessionConstants.NICKNAME);
            }

            set
            {
                SessionExtension.Set(SessionConstants.NICKNAME, value);
            }
        }

        public static string EncryptPassword
        {
            get
            {
                return SessionExtension.Get<string>(SessionConstants.ENCRYPTPASSWORD);
            }

            set
            {
                SessionExtension.Set(SessionConstants.ENCRYPTPASSWORD, value);
            }
        }

        public static int SuperId
        {
            get
            {
                return SessionExtension.Get<int>(SessionConstants.SUPERID);
            }

            set
            {
                SessionExtension.Set(SessionConstants.SUPERID, value);
            }
        }

        public static string Site
        {
            get
            {
                return SessionExtension.Get<string>(SessionConstants.SITE) ?? string.Empty;
            }

            set
            {
                SessionExtension.Set(SessionConstants.SITE, value);
            }
        }

        public static byte RoleId
        {
            get
            {
                return SessionExtension.Get<byte>(SessionConstants.ROLEID);
            }

            set
            {
                SessionExtension.Set(SessionConstants.ROLEID, value);
            }
        }

        public static string RobotResult
        {
            get
            {
                return SessionExtension.Get<string>(SessionConstants.ROBOTRESULT);
            }

            set
            {
                SessionExtension.Set(SessionConstants.ROBOTRESULT, value);
            }
        }

        public static int RobotCaptchaFailedCount
        {
            get
            {
                return SessionExtension.Get<int>(SessionConstants.ROBOTCAPTCHAFAILEDCOUNT);
            }

            set
            {
                SessionExtension.Set(SessionConstants.ROBOTCAPTCHAFAILEDCOUNT, value);
            }
        }

        public static string DeviceId
        {
            get
            {
                return SessionExtension.Get<string>(SessionConstants.DEVICEID);
            }

            set
            {
                SessionExtension.Set(SessionConstants.DEVICEID, value);
            }
        }

        public static string BrowserSize
        {
            get
            {
                return SessionExtension.Get<string>(SessionConstants.BROWSERSIZE);
            }

            set
            {
                SessionExtension.Set(SessionConstants.BROWSERSIZE, value);
            }
        }

        public static string AssetAppPath
        {
            get
            {
                return SessionExtension.Get<string>(SessionConstants.ASSETAPPPATH);
            }

            set
            {
                SessionExtension.Set(SessionConstants.ASSETAPPPATH, value);
            }
        }

        public static bool InputCaptchaSuccess
        {
            get
            {
                return SessionExtension.Get<bool>(SessionConstants.INPUTCAPTCHASUCCESS);
            }

            set
            {
                SessionExtension.Set(SessionConstants.INPUTCAPTCHASUCCESS, value);
            }
        }

        public static void AssignRobotCaptchaSession(
            int custId,
            string username,
            int subAccId,
            string subAccName,
            bool isNickname,
            string nickname,
            string encryptPassword,
            int superId,
            string site,
            byte roleId,
            string robotResult,
            string deviceId,
            string browserSize,
            string assetAppPath)
        {
            CustId = custId;
            Username = username;
            SubAccId = subAccId;
            SubAccName = subAccName;
            IsNickname = isNickname;
            Nickname = nickname;
            EncryptPassword = encryptPassword;
            SuperId = superId;
            Site = site;
            RoleId = roleId;

            RobotResult = robotResult;
            DeviceId = deviceId;
            BrowserSize = browserSize;
            AssetAppPath = assetAppPath;
        }

        public static void ResetRobotCaptchaSession()
        {
            CustId = 0;
            Username = null;
            SubAccId = 0;
            SubAccName = null;
            IsNickname = false;
            Nickname = null;
            EncryptPassword = null;
            SuperId = 0;
            Site = null;
            RoleId = 0;

            RobotResult = null;
            RobotCaptchaFailedCount = 0;
            DeviceId = null;
            BrowserSize = null;
            InputCaptchaSuccess = false;
        }

        public static void IncreaseRobotCaptchaFailedCount()
        {
            if (RobotCaptchaFailedCount < int.MaxValue)
            {
                RobotCaptchaFailedCount++;
            }
        }

        #endregion
    }
}
