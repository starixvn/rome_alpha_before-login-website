// -----------------------------------------------------------------------
// <copyright file="Result.cs" company="Nexcel Solution">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Razorbill.Sphinx.Core.Models
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Result
    {
        public Result()
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;
        }

        public int ErrorCode { get; set; }

        public string ErrorMessage { get; set; }
    }
}
