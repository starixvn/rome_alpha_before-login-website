namespace Fanex.Alpha.Before.Customer.Repositories.SignIn
{
    using System.Collections;
    using Fanex.Alpha.Before.Customer.Models;

    public interface ISignInRepository
    {
        VerifySignInResult VerifyDownlineLogin(string username, string password, Hashtable requestInfo, bool? isDarkMode);
    }
}
