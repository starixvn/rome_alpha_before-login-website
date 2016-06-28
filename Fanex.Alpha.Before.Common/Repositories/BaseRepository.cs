// --------------------------------------------------------------------------
// <copyright file="BaseRepository.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.Alpha.Before.Common.Repositories
{
    using Fanex.Alpha.Before.Infrastructure.Models;
    using Fanex.Common;
    using Fanex.Data;
    using Fanex.Data.Repository;

    /// <summary>
    /// Base Repository.
    /// </summary>
    public class BaseRepository
    {
        /// <summary>
        /// Runs the store procedure.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Run SP result.</returns>
        public Result RunSP(CriteriaBase criteria)
        {
            Result runResult = null;
            using (var objectDB = new ObjectDb(criteria.GetSettingKey()))
            {
                objectDB.AssignParamValues(criteria.GetSpParams());
                objectDB.ExecuteNonQuery();
                var outputParams = objectDB.GetOuputParams();
                runResult = new Result
                {
                    Code = Converter.ToInt(outputParams["errcode"]),
                    Message = Converter.ToString2(outputParams["errmsg"])
                };
            }

            return runResult;
        }
    }
}
