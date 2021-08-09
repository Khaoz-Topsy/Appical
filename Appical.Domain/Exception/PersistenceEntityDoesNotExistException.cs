using System;

namespace Appical.Domain.Exception
{
    public class PersistenceEntityDoesNotExistException : System.Exception
    {
        public Guid Id { get; set; }

        public PersistenceEntityDoesNotExistException()
        {
        }

        public PersistenceEntityDoesNotExistException(Guid entityId)
        {
            Id = entityId;
        }
    }
}
