using System;
using System.Collections.Generic;
using Appical.Domain.Dto.Account;
using Appical.Domain.Dto.Transaction;
using Appical.Domain.Enum;
using Appical.Persistence.Entity;

namespace Appical.Persistence.Mapper
{
    public static class AccountMapper
    {
        public static Account ToPersistence(AccountDto dto)
        {
            return new Account
            {
                Id = dto.Id,
                AccountOwnerId = dto.AccountOwnerId,
                Balance = dto.Balance,
                Status = dto.Status,
                ClosureReason = dto.ClosureReason,
                ClosureDate = dto.ClosureDate,
            };
        }

        public static AccountDto Create(Guid accountOwnerId)
        {
            AccountDto dtoToCreate = new AccountDto
            {
                Id = Guid.NewGuid(),
                AccountOwnerId = accountOwnerId,
                Balance = 0,
                Status = AccountStatus.Open,
            };

            return dtoToCreate;
        }

        public static Account EditExisting(Account persistence, AccountDto dto)
        {
            persistence.Balance = dto.Balance;
            persistence.Status = dto.Status;
            persistence.ClosureReason = dto.ClosureReason;
            persistence.ClosureDate = dto.ClosureDate;

            return persistence;
        }

        public static AccountDto ToDto(Account persistence)
        {
            return new AccountDto
            {
                Id = persistence.Id,
                AccountOwnerId = persistence.AccountOwnerId,
                Balance = persistence.Balance,
                Status = persistence.Status,
                ClosureReason = persistence.ClosureReason,
                ClosureDate = persistence.ClosureDate,
            };
        }

        public static AccountOverviewDto ToOverviewDto(Account persistence, List<TransactionDto> transactions)
        {
            return new AccountOverviewDto
            {
                Id = persistence.Id,
                AccountOwnerId = persistence.AccountOwnerId,
                Balance = persistence.Balance,
                Status = persistence.Status,
                ClosureReason = persistence.ClosureReason,
                ClosureDate = persistence.ClosureDate,
                Transactions = transactions,
            };
        }
    }
}
