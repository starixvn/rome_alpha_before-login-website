using System.Collections;
using Fanex.Alpha.Before.Extensions;
using Fanex.Alpha.Before.Models;
using Fanex.Common;
using Fanex.Data;

namespace Fanex.Alpha.Before.Repositories
{
    public interface ISignInRepository
    {
        VerifySignInResult VerifyDownlineLogin(string username, string password, Hashtable requestInfo, bool? isDarkMode);
    }

    public class SignInRepository : ISignInRepository
    {
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
