using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appical.Domain.Dto.Account;
using Appical.Domain.Dto.Transaction;
using Appical.Domain.Enum;
using Appical.Domain.Exception;
using Appical.Persistence.Entity;
using Appical.Persistence.Mapper;
using Appical.Persistence.Repository.Interface;
using Appical.Persistence.Validator;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Appical.Persistence.Repository
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppicalContext _db;

        public TransactionRepository(AppicalContext db)
        {
            _db = db;
        }

        /// <inheritdoc cref="Create(TransactionDto)" />
        /// <summary>
        /// Create Transaction with default properties set
        /// </summary>
        /// <param name="transactionDto">Reduced amount of properties required for creating a transaction</param>
        public Task<TransactionDto> Create(AccountTransactionDto transactionDto) => Create(TransactionMapper.Create(transactionDto));

        /// <summary>
        /// Create a Transaction
        /// </summary>
        /// <param name="dto"></param>
        /// <exception cref="PersistenceEntityDoesNotExistException">Account doesn't exist for dto.Id.</exception>
        /// <exception cref="AccountClosedException">Transactions cannot be performed on a closed Account.</exception>
        /// <exception cref="InvalidAccountOperationException">Processing this transaction would lead to negative account balance.</exception>
        public async Task<TransactionDto> Create(TransactionDto dto)
        {
            Account accountToManipulate = await _db.Accounts.FirstOrDefaultAsync(acc => acc.Id.Equals(dto.AccountId));
            if (accountToManipulate == null) throw new PersistenceEntityDoesNotExistException(dto.AccountId);
            if (accountToManipulate.Status == AccountStatus.Closed) throw new AccountClosedException(dto.AccountId);

            Transaction latestTransaction = await _db.Transactions
                .OrderByDescending(t => t.ActionDate)
                .FirstOrDefaultAsync();
            if (latestTransaction != null) // Only validate previousTransactionId if there is a latestTransaction
            {
                if (!latestTransaction.Id.Equals(dto.PreviousTransactionId)) throw new InvalidAccountOperationException(dto.AccountId);
            }

            dto.Validate(accountToManipulate.Balance);

            IDbContextTransaction transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                accountToManipulate.Balance += dto.Amount;
                _db.Accounts.Update(accountToManipulate);

                Transaction persistence = TransactionMapper.ToPersistence(dto);
                await _db.Transactions.AddAsync(persistence);

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return TransactionMapper.ToDto(persistence);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw; // Throw original exception
            }
        }
        
        /// <summary>
        /// Implements <see cref="ITransactionRepository.Read()"/>
        /// Get all transactions in the database
        /// </summary>
        public async Task<List<TransactionDto>> Read()
        {
            List<Transaction> accountOwners = await _db.Transactions
                .OrderByDescending(t => t.ActionDate)
                .ToListAsync();
            return accountOwners.Select(TransactionMapper.ToDto).ToList();
        }

        /// <summary>
        /// Implements <see cref="ITransactionRepository.Read(Guid)"/>
        /// Get specific transaction
        /// </summary>
        /// <param name="id"></param>
        public async Task<TransactionDto> Read(Guid id)
        {
            Transaction existingPersistence = await _db.Transactions.FirstOrDefaultAsync(tran => tran.Id.Equals(id));
            if (existingPersistence == null) throw new PersistenceEntityDoesNotExistException(id);

            return TransactionMapper.ToDto(existingPersistence);
        }

        /// <summary>
        /// Get latest transaction for AccountId specified
        /// </summary>
        /// <param name="accountId"></param>
        public async Task<TransactionDto> GetLatestTransactionId(Guid accountId)
        {
            Transaction existingPersistence = await _db.Transactions
                .Where(tran => tran.AccountId.Equals(accountId))
                .OrderByDescending(t => t.ActionDate)
                .FirstOrDefaultAsync();
            if (existingPersistence == null) throw new PersistenceEntityDoesNotExistException(accountId);

            return TransactionMapper.ToDto(existingPersistence);
        }

        /// <summary>
        /// Get transactions for a specific Account
        /// </summary>
        /// <param name="accountId"></param>
        public async Task<AccountOverviewDto> ReadForAccount(Guid accountId)
        {
            Account existingAccount = await _db.Accounts.FirstOrDefaultAsync(ao => ao.Id.Equals(accountId));
            if (existingAccount == null) throw new PersistenceEntityDoesNotExistException(accountId);

            List<Transaction> existingPersistence = await _db.Transactions
                .Where(tran => tran.AccountId.Equals(accountId))
                .OrderByDescending(tran => tran.ActionDate)
                .ToListAsync();

            List<TransactionDto> transactions = existingPersistence.Select(TransactionMapper.ToDto).ToList();
            return AccountMapper.ToOverviewDto(existingAccount, transactions);
        }

        /// <summary>
        /// Implements <see cref="ITransactionRepository.Update"/>
        /// Update existing Transaction
        /// </summary>
        /// <param name="dto"></param>
        /// <exception cref="AccountClosedException">Transactions cannot be performed on a closed Account.</exception>
        /// <exception cref="PersistenceEntityDoesNotExistException">Transaction doesn't exist for dto.Id.</exception>
        public async Task<TransactionDto> Update(TransactionDto dto)
        {
            Transaction existingPersistence = await _db.Transactions.FirstOrDefaultAsync(tran => tran.Id.Equals(dto.Id));
            if (existingPersistence == null) throw new PersistenceEntityDoesNotExistException(dto.Id);

            Account accountPersistence = await _db.Accounts.FirstOrDefaultAsync(acc => acc.Id.Equals(existingPersistence.AccountId));
            if (accountPersistence == null) throw new PersistenceEntityDoesNotExistException(existingPersistence.AccountId);
            if (accountPersistence.Status == AccountStatus.Closed) throw new AccountClosedException(existingPersistence.AccountId);

            dto.Validate(accountPersistence.Balance);

            Transaction alteredPersistence = TransactionMapper.EditExisting(existingPersistence, dto);
            _db.Transactions.Update(alteredPersistence);
            await _db.SaveChangesAsync();

            return TransactionMapper.ToDto(alteredPersistence);
        }

        /// <summary>
        /// Implements <see cref="ITransactionRepository.Delete"/>
        /// Delete existing Transaction. Changes will be undone if any exception is thrown
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="PersistenceEntityDoesNotExistException">Transaction or Account doesn't exist for dto.Id or dto.AccountId respectively.</exception>
        public async Task<TransactionDto> Delete(Guid id)
        {
            IDbContextTransaction transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                Transaction existingPersistence = await _db.Transactions.FirstOrDefaultAsync(tran => tran.Id.Equals(id));
                if (existingPersistence == null) throw new PersistenceEntityDoesNotExistException(id);

                // Adjust account balance after removing a transaction
                Account accountPersistence = await _db.Accounts.FirstOrDefaultAsync(acc => acc.Id.Equals(existingPersistence.AccountId));
                if (accountPersistence == null) throw new PersistenceEntityDoesNotExistException(existingPersistence.AccountId);
                if (accountPersistence.Status == AccountStatus.Closed) throw new AccountClosedException(existingPersistence.AccountId);

                if (existingPersistence.Action == TransactionActionType.Deposit)
                    accountPersistence.Balance -= existingPersistence.Amount;
                else
                    accountPersistence.Balance += existingPersistence.Amount;

                _db.Accounts.Update(accountPersistence);
                _db.Transactions.Remove(existingPersistence);

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                return TransactionMapper.ToDto(existingPersistence);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw; // Throw original exception
            }
        }
    }
}
