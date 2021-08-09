using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Appical.Domain.Dto.Account;
using Appical.Domain.Dto.Transaction;

namespace Appical.Persistence.Repository.Interface
{
    public interface ITransactionRepository: ICrudRepository<TransactionDto>
    {
        /// <summary>
        /// Create Transaction with default properties
        /// </summary>
        /// <param name="transactionDto"></param>
        Task<TransactionDto> Create(AccountTransactionDto transactionDto);

        /// <summary>
        /// Get latest transaction for AccountId specified
        /// </summary>
        /// <param name="accountId"></param>
        Task<Guid?> GetLatestTransactionId(Guid accountId);

        /// <summary>
        /// Get transaction list for AccountIds specified
        /// </summary>
        /// <param name="accountId"></param>
        Task<AccountOverviewDto> ReadForAccount(Guid accountId);
    }
}
