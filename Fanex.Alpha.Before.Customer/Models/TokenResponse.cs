namespace Fanex.Alpha.Before.Customer.Models
{
    using System.Collections.Generic;

    public class TokenResponse
    {
        public bool Success { get; set; }

        public decimal Score { get; set; }

        public string Action { get; set; }

        public List<string> ErrorCodes { get; set; }
    }
}
