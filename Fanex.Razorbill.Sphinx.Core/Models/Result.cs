// -----------------------------------------------------------------------
// <copyright file="Result.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Razorbill.Sphinx.Core.Models
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public class Result
    {
        public int ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

        // public dynamic ExtendContent { get; set; }

        public Result()
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;
        }
    }
}
