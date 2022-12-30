// -----------------------------------------------------------------------
// <copyright file="SignInRepository.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Alpha.Before.Customer.Repositories.SignIn
{
    using System;
    using System.Collections;
    using Fanex.Alpha.Before.Common.Configurations;
    using Fanex.Alpha.Before.Common.Extensions;
    using Fanex.Alpha.Before.Common.Helpers;
    using Fanex.Alpha.Before.Customer.Models;
    using Fanex.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SignInRepository : ISignInRepository
    {
        /// <summary>
        /// Verify Down-line Login.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">User's password.</param>
        /// <param name="requestInfo">The request information.</param>
        /// <returns>Return verify result.</returns>
        public VerifySignInResult VerifyDownlineLogin(string username, string password, Hashtable requestInfo, bool? isDarkMode)
        {
            using var objectDB = ObjectDbFactory.CreateInstance("Age_LoginVerifyDownline");
            Hashtable parameters = new()
            {
                    { "username", username },
                    { "userpwd", EncryptExtension.CfsEnCode(password) },
                    { "custid", 0 },
                    { "subaccid", 0 },
                    { "serverid", 0 },
                    { "logintime", DateTime.Now },
                    { "language", AppSession.LangKey },
                    { "isnickname", false },
                    { "nickname", string.Empty },
                    { "errcode", 0 },
                    { "errmsg", string.Empty },
                    { "lastchangedpwddate", null },
                    { "superid", 0 },
                    { "site", string.Empty },
                    { "roleid", 0 },
                    { "subusername", string.Empty },
                    { "iskickout", false },
                    { "isdarkmode", isDarkMode }
                };
            InsertRequestInfo(requestInfo, parameters);

            objectDB.AssignParamValues(parameters);
            objectDB.ExecuteNonQuery();
            var outputParams = objectDB.GetOutputParams();

            return new VerifySignInResult
            {
                CustId = Converter.ToInt(outputParams["custid"]),
                Username = username,
                SubAccId = Converter.ToInt(outputParams["subaccid"]),
                IsNickname = Converter.ToBool(outputParams["isnickname"]),
                Nickname = Converter.ToString2(outputParams["nickname"]),
                LoginTime = Converter.ToDateTime(outputParams["logintime"]),
                LastTimeToChangePassword = outputParams["lastchangedpwddate"] != null ?
                    Converter.ToDateTime(outputParams["lastchangedpwddate"]) :
                    DateTime.MinValue,
                ErrorCode = Converter.ToInt(outputParams["errcode"]),
                ErrorMessage = Converter.ToString2(outputParams["errmsg"]),
                SuperId = Converter.ToInt(outputParams["superid"]),
                Site = Converter.ToString2(outputParams["site"]),
                RoleId = Converter.To<Byte>(outputParams["roleid"]),
                SubUserName = Converter.ToString2(outputParams["subusername"]),
                ServerId = Converter.ToInt(outputParams["serverid"]),
                IsKickoutDueToProtectedMode = Converter.ToBool(outputParams["iskickout"])
            };

        }

        private static void InsertRequestInfo(Hashtable requestInfo, Hashtable parameters)
        {
            foreach (DictionaryEntry d in requestInfo)
            {
                parameters.Add(d.Key, d.Value);
            }
        }
    }
}
