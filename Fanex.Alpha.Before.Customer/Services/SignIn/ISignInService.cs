// -----------------------------------------------------------------------
// <copyright file="ISignInService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Alpha.Before.Customer.Services.SignIn
{
    using System.Drawing;
    using Fanex.Alpha.Before.Customer.Models;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface ISignInService
    {
        SignInResult SignIn();

        SignInResult SignIn(string username, string password, string language, string captcha, string code);

        Graphics NewCaptcha();

        SignInResult AutomationTest(string username, string password, string captcha);
    }
}
