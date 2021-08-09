using System.Collections.Generic;
using Appical.Domain.Dto.Account;
using Appical.Domain.Exception;

namespace Appical.Persistence.Validator
{
    public static class AccountValidator
    {
        /// <exception cref="PersistenceEntityNotValidException">Issues found during validation.</exception>
        public static void Validate(this AccountDto dto)
        {
            List<string> validationIssues = new List<string>();
            validationIssues.AddRange(BaseValidator.ValidateGuid(dto.Id));
            validationIssues.AddRange(BaseValidator.ValidateDecimalNotNegative(dto.Balance, "Balance"));

            if (validationIssues.Count > 0) throw new PersistenceEntityNotValidException(validationIssues);
        }
    }
}
