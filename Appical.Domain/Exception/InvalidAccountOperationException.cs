using System;

namespace Appical.Domain.Exception
{
    public class InvalidAccountOperationException : System.Exception
    {
        public Guid Id { get; set; }

        public InvalidAccountOperationException()
        {
        }

        public InvalidAccountOperationException(Guid id)
        {
            Id = id;
        }
    }
}
