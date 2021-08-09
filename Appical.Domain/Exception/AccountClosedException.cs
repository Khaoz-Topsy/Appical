using System;

namespace Appical.Domain.Exception
{
    public class AccountClosedException : System.Exception
    {
        public Guid Id { get; set; }

        public AccountClosedException()
        {
        }

        public AccountClosedException(Guid id)
        {
            Id = id;
        }
    }
}
