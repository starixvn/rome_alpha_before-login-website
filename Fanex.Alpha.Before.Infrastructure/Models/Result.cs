// --------------------------------------------------------------------------
// <copyright file="Result.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.Alpha.Before.Infrastructure.Models
{
    using System;

    /// <summary>
    /// Result Model.
    /// </summary>
    [Serializable]
    public class Result
    {
        public Result()
        {
        }

        public Result(int code, string message)
        {
            Code = code;
            Message = message;
        }

        public int Code { get; set; }

        public string Message { get; set; }
    }
}
