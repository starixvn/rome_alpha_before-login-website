// --------------------------------------------------------------------------
// <copyright file="BaseModel.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.Alpha.Before.Infrastructure.Models
{
    using System;

    /// <summary>
    /// Base Model.
    /// </summary>
    [Serializable]
    public class BaseModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string AliasName { get; set; }
    }
}
