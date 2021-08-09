using System.Collections.Generic;

namespace Appical.Domain.Exception
{
    public class PersistenceEntityNotValidException : System.Exception
    {
        public List<string> Messages { get; set; }

        public PersistenceEntityNotValidException(List<string> messages)
        {
            Messages = messages;
        }
    }
}
