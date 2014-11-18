// -----------------------------------------------------------------------
// <copyright file="LimitedCountryAccessService.cs" company="Nexcel Solution">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Razorbill.Sphinx.Service.Common.LimitedCountryAccess
{
    using System.Collections;
    using Fanex.Common;
    using Fanex.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LimitedCountryAccessService
    {
        public LimitedCountryAccessService()
        {
        }

        /// <summary>
        /// Checking Limited country access.
        /// </summary>
        /// <returns>Return result.</returns>
        public bool LimitedAccess()
        {
            using (ObjectDb objectDb = new ObjectDb("Age_CheckValidCountry"))
            {
                objectDb.AssignParamValues(new Hashtable()
                {
                    { "ip", Fanex.Razorbill.Sphinx.Core.Extensions.IPExtension.CustomerIp() },
                    { "isvalid", 1 }
                });

                objectDb.ExecuteNonQuery();

                return !Converter.ToBool(objectDb.GetOuputParams()["isvalid"]);
            }
        }
    }
}
