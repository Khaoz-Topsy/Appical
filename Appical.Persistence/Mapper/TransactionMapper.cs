using System;
using Appical.Domain.Dto.Account;
using Appical.Domain.Dto.Transaction;
using Appical.Domain.Enum;
using Appical.Persistence.Entity;

namespace Appical.Persistence.Mapper
{
    public static class TransactionMapper
    {
        public static Transaction ToPersistence(TransactionDto dto)
        {
            return new Transaction
            {
                Id = dto.Id,
                AccountId = dto.AccountId,
                Amount = Math.Abs(dto.Amount),
                Action = (dto.Amount > 0) ? TransactionActionType.Deposit : TransactionActionType.Withdrawal,
                ActionDate = dto.ActionDate,
            };
        }

        public static TransactionDto Create(AccountTransactionDto transactionDto)
        {
            TransactionDto dtoToCreate = new TransactionDto
            {
                Id = Guid.NewGuid(),
                AccountId = transactionDto.AccountId,
                Amount = transactionDto.Amount,
                PreviousTransactionId = transactionDto.PreviousTransactionId,
                ActionDate = DateTime.Now,
            };

            return dtoToCreate;
        }

        public static Transaction EditExisting(Transaction persistence, TransactionDto dto)
        {
            persistence.Amount = Math.Abs(dto.Amount);
            persistence.Action = (dto.Amount > 0) ? TransactionActionType.Deposit : TransactionActionType.Withdrawal;
            persistence.ActionDate = dto.ActionDate;

            return persistence;
        }

        public static TransactionDto ToDto(Transaction persistence)
        {
            return new TransactionDto
            {
                Id = persistence.Id,
                AccountId = persistence.AccountId,
                Amount = (persistence.Action == TransactionActionType.Deposit) ? persistence.Amount : Decimal.Negate(persistence.Amount),
                ActionDate = persistence.ActionDate,
            };
        }
    }
}
