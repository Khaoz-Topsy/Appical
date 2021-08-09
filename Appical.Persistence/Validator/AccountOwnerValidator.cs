using System.Collections.Generic;
using Appical.Domain.Dto.AccountOwner;
using Appical.Domain.Exception;
using Appical.Persistence.Entity;

namespace Appical.Persistence.Validator
{
    public static class AccountOwnerValidator
    {
        /// <exception cref="PersistenceEntityNotValidException">Issues found during validation.</exception>
        public static void Validate(this AccountOwnerDto dto)
        {
            List<string> validationIssues = new List<string>();
            validationIssues.AddRange(BaseValidator.ValidateGuid(dto.Id));
            validationIssues.AddRange(BaseValidator.ValidateStringLength(dto.Name, AccountOwner.NameMaxLength, "Name"));

            if (validationIssues.Count > 0) throw new PersistenceEntityNotValidException(validationIssues);
        }
    }
}
