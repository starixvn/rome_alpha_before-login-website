// -----------------------------------------------------------------------
// <copyright file="ErrorContants.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Razorbill.Sphinx.Core.Constants
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public static class ErrorContants
    {
        public const int ErrorInvalidLogin = 101;
        public const int ErrorInvalidCaptcha = 102;
        public const int Error180 = 108;

        public const int Error302 = 302;
        
        public const int Error403 = 403;
        public const int Error404 = 404;

        public const int ErrorDefault = 500;

        public const int ErrorInvalidCode909 = 909;

        public const int ErrorUMT = 1000;
    }
}
