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

        /// <summary>
        /// Sign in with new captcha.
        /// </summary>
        /// <param name="username">The user name string.</param>
        /// <param name="password">The password string.</param>
        /// <param name="language">The language string.</param>
        /// <param name="captcha">The captcha string.</param>
        /// <param name="code">The code string.</param>
        /// <returns>Sign In result.</returns>
        SignInResult SignIn(string username, string password, string language, string captcha, string code);

        /// <summary>
        /// Signs the in with re-captcha.
        /// </summary>
        /// <param name="username">The username string.</param>
        /// <param name="password">The password string.</param>
        /// <param name="language">The language string.</param>
        /// <param name="isValidCaptcha">If set to <c>true</c> [is valid captcha].</param>
        /// <returns>Sign In result.</returns>
        SignInResult SignInWithRecaptcha(string username, string password, string language, bool isValidCaptcha);

        Graphics NewCaptcha();

        SignInResult AutomationTest(string username, string password, string captcha);

        bool IsShowReCaptcha();
    }
}
