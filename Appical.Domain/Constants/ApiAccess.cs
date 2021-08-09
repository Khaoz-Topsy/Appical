using System.Collections.Generic;

namespace Appical.Domain.Constants
{
    public static class ApiAccess
    {
        public static string All = "all";
        public static string BankClerk = "bankclerk";
        public static string AccountOwner = "accountowner";

        public static List<string> BankClerkControllers = new List<string>
        {
            "BankClerk",
            "Transaction",
        };

        public static List<string> AccountOwnerControllers = new List<string>
        {
            "Account",
            "AccountOwner",
            "Transaction",
        };
    }
}
