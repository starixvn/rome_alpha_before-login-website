// --------------------------------------------------------------------------
// <copyright file="GuidExtension.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.Alpha.Before.Common.Extensions
{
    using System;
    using System.Diagnostics;
    using Fanex.Alpha.Before.Common.Helpers;

    /// <summary>
    /// Globally unique identifier Extension.
    /// </summary>
    public static class GuidExtension
    {
        [DebuggerStepThrough]
        public static string Shrink(this Guid target)
        {
            string base64 = Convert.ToBase64String(target.ToByteArray());
            string encoded = base64.Replace("/", "_").Replace("+", "-");
            return encoded.Substring(0, 22);
        }

        [DebuggerStepThrough]
        public static bool IsEmpty(this Guid target)
        {
            return target == Guid.Empty;
        }
    }
}