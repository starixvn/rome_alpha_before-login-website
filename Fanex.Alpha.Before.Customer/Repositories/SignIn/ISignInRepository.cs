// -----------------------------------------------------------------------
// <copyright file="ISignInRepository.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Alpha.Before.Customer.Repositories.SignIn
{
    using System.Collections;
    using Fanex.Alpha.Before.Customer.Models;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface ISignInRepository
    {
        VerifySignInResult VerifyDownlineLogin(string username, string password, Hashtable requestInfo);
    }
}
