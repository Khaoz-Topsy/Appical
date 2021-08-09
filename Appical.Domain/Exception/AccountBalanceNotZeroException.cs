using System;
using System.Collections.Generic;

namespace Appical.Domain.Exception
{
    public class AccountBalanceNotZeroException : System.Exception
    {
        public List<Guid> AccountIds { get; set; }

        public AccountBalanceNotZeroException()
        {
        }

        public AccountBalanceNotZeroException(List<Guid> accountIds)
        {
            AccountIds = accountIds;
        }
    }
}
