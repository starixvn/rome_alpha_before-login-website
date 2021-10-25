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
    using Fanex.Alpha.Before.Customer.Models;
    using Fanex.Common;
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
        public VerifySignInResult VerifyDownlineLogin(string username, string password, Hashtable requestInfo)
        {
            using (var objectDB = new ObjectDb("Age_LoginVerifyDownline"))
            {
                Hashtable parameters = new Hashtable();
                parameters.Add("username", username);
                parameters.Add("userpwd", EncryptExtension.CfsEnCode(password));
                parameters.Add("custid", 0);
                parameters.Add("subaccid", 0);
                parameters.Add("serverid", 0);
                parameters.Add("logintime", DateTime.Now);
                parameters.Add("language", AppSession.LangKey);
                parameters.Add("isnickname", false);
                parameters.Add("nickname", string.Empty);
                parameters.Add("errcode", 0);
                parameters.Add("errmsg", string.Empty);
                parameters.Add("lastchangedpwddate", null);
                parameters.Add("superid", 0);
                parameters.Add("site", string.Empty);
                parameters.Add("roleid", 0);
                parameters.Add("subusername", string.Empty);
                parameters.Add("iskickout", false);
                InsertRequestInfo(requestInfo, parameters);

                objectDB.AssignParamValues(parameters);
                objectDB.ExecuteNonQuery();
                var outputParams = objectDB.GetOuputParams();

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
