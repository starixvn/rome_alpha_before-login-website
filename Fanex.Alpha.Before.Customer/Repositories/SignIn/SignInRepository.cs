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
                InsertRequestInfo(requestInfo, parameters);

                objectDB.AssignParamValues(parameters);
                objectDB.ExecuteNonQuery();
                var result = new VerifySignInResult
                {
                    CustId = Converter.ToInt(objectDB.GetOuputParams()["custid"]),
                    Username = Converter.ToString2(objectDB.GetOuputParams()["username"]),
                    SubAccId = Converter.ToInt(objectDB.GetOuputParams()["subaccid"]),
                    IsNickname = Converter.ToBool(objectDB.GetOuputParams()["isnickname"]),
                    Nickname = Converter.ToString2(objectDB.GetOuputParams()["nickname"]),
                    LoginTime = Converter.ToDateTime(objectDB.GetOuputParams()["logintime"]),
                    LastTimeToChangePassword = objectDB.GetOuputParams()["lastchangedpwddate"] != null ?
                        Converter.ToDateTime(objectDB.GetOuputParams()["lastchangedpwddate"]) :
                        DateTime.MinValue,
                    ErrorCode = Converter.ToInt(objectDB.GetOuputParams()["errcode"]),
                    ErrorMessage = Converter.ToString2(objectDB.GetOuputParams()["errmsg"])
                };

                return result;
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
