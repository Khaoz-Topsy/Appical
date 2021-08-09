using System;

namespace Appical.Domain.Exception
{
    public class PersistenceEntityAlreadyExistsException: System.Exception
    {
        public Guid Id { get; set; }

        public PersistenceEntityAlreadyExistsException()
        {
        }

        public PersistenceEntityAlreadyExistsException(Guid entityId)
        {
            Id = entityId;
        }
    }
}
