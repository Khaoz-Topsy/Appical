using System.Collections.Generic;
using Appical.Domain.Dto.Transaction;
using Appical.Domain.Exception;

namespace Appical.Persistence.Validator
{
    public static class TransactionValidator
    {
        /// <exception cref="PersistenceEntityNotValidException">Issues found during validation.</exception>
        public static void Validate(this TransactionDto dto, decimal currentBalance)
        {
            List<string> validationIssues = new List<string>();
            validationIssues.AddRange(BaseValidator.ValidateGuid(dto.Id));
            validationIssues.AddRange(BaseValidator.ValidateGuid(dto.AccountId));
            validationIssues.AddRange(BaseValidator.ValidateDateNotFuture(dto.ActionDate, "ActionDate"));

            decimal newBalance = currentBalance + dto.Amount;
            validationIssues.AddRange(BaseValidator.ValidateDecimalNotNegative(newBalance, "NewBalance"));

            if (validationIssues.Count > 0) throw new PersistenceEntityNotValidException(validationIssues);
        }
    }
}
