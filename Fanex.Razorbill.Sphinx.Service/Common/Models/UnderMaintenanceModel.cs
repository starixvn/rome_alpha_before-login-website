// -----------------------------------------------------------------------
// <copyright file="UnderMaintenanceModel.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Razorbill.Sphinx.Service.Common.Models
{

    using System.Diagnostics.CodeAnalysis;
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public class UnderMaintenanceModel
    {
        public bool IsUM { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }
}
