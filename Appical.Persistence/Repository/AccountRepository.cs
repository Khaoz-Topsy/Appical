using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appical.Domain.Dto.Account;
using Appical.Domain.Enum;
using Appical.Domain.Exception;
using Appical.Persistence.Entity;
using Appical.Persistence.Mapper;
using Appical.Persistence.Repository.Interface;
using Appical.Persistence.Validator;
using Microsoft.EntityFrameworkCore;

namespace Appical.Persistence.Repository
{
    public class AccountRepository: IAccountRepository
    {
        private readonly AppicalContext _db;

        public AccountRepository(AppicalContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Create an Account with default properties
        /// </summary>
        /// <param name="accountOwnerId"></param>
        public Task<AccountDto> Create(Guid accountOwnerId) => Create(AccountMapper.Create(accountOwnerId));

        /// <summary>
        /// Create an Account
        /// </summary>
        /// <param name="dto"></param>
        public async Task<AccountDto> Create(AccountDto dto)
        {
            dto.Validate();

            Account persistence = AccountMapper.ToPersistence(dto);
            await _db.Accounts.AddAsync(persistence);
            await _db.SaveChangesAsync();

            return AccountMapper.ToDto(persistence);
        }

        /// <summary>
        /// Implements <see cref="IAccountRepository.Read()"/>
        /// Get all Accounts that exist
        /// </summary>
        public async Task<List<AccountDto>> Read()
        {
            List<Account> accountOwners = await _db.Accounts.ToListAsync();
            return accountOwners.Select(AccountMapper.ToDto).ToList();
        }

        /// <summary>
        /// Implements <see cref="IAccountRepository.Read(Guid)"/>
        /// Get specific Account
        /// </summary>
        /// <param name="id"></param>
        public async Task<AccountDto> Read(Guid id)
        {
            Account existingPersistence = await _db.Accounts.FirstOrDefaultAsync(ao => ao.Id.Equals(id));
            if (existingPersistence == null) throw new PersistenceEntityDoesNotExistException(id);

            return AccountMapper.ToDto(existingPersistence);
        }

        /// <summary>
        /// Get Accounts for a AccountOwner
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="PersistenceEntityDoesNotExistException">AccountOwner does not exist for given Id.</exception>
        public async Task<List<AccountDto>> ReadForAccountOwner(Guid id)
        {
            List<Account> existingPersistence = await _db.Accounts.Where(ao => ao.AccountOwnerId.Equals(id)).ToListAsync();
            if (existingPersistence == null) throw new PersistenceEntityDoesNotExistException(id);

            return existingPersistence.Select(AccountMapper.ToDto).ToList();
        }

        /// <summary>
        /// Implements <see cref="IAccountRepository.Update"/>
        /// update account for the given Id
        /// </summary>
        /// <param name="dto"></param>
        /// <exception cref="PersistenceEntityDoesNotExistException">Account does not exist for given Id.</exception>
        public async Task<AccountDto> Update(AccountDto dto)
        {
            Account existingPersistence = await _db.Accounts.FirstOrDefaultAsync(ao => ao.Id.Equals(dto.Id));
            if (existingPersistence == null) throw new PersistenceEntityDoesNotExistException(dto.Id);

            dto.Validate();

            Account alteredPersistence = AccountMapper.EditExisting(existingPersistence, dto);
            _db.Accounts.Update(alteredPersistence);
            await _db.SaveChangesAsync();

            return AccountMapper.ToDto(alteredPersistence);
        }

        /// <summary>
        /// Implements <see cref="IAccountRepository.Delete"/>
        /// Delete account only if the balance is zero
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="PersistenceEntityDoesNotExistException">Account does not exist for given Id.</exception>
        /// <exception cref="AccountBalanceNotZeroException">Account contains funds and therefore cannot be closed.</exception>
        public async Task<AccountDto> Delete(Guid id)
        {
            Account existingPersistence = await _db.Accounts.FirstOrDefaultAsync(ao => ao.Id.Equals(id));
            if (existingPersistence == null) throw new PersistenceEntityDoesNotExistException(id);
            
            if (existingPersistence.Balance > 0)
            {
                throw new AccountBalanceNotZeroException(new List<Guid>{ existingPersistence.Id });
            }

            _db.Accounts.Remove(existingPersistence);
            await _db.SaveChangesAsync();

            return AccountMapper.ToDto(existingPersistence);
        }

        /// <summary>
        /// Set account to closed
        /// </summary>
        /// <param name="id"></param>
        /// <param name="closureReason"></param>
        /// <exception cref="PersistenceEntityDoesNotExistException">Account does not exist for given Id.</exception>
        /// <exception cref="AccountBalanceNotZeroException">Account contains funds and therefore cannot be closed.</exception>
        public async Task<AccountDto> CloseAccount(Guid id, ClosureReasonType closureReason)
        {
            Account existingPersistence = await _db.Accounts.FirstOrDefaultAsync(ao => ao.Id.Equals(id));
            if (existingPersistence == null) throw new PersistenceEntityDoesNotExistException(id);

            if (existingPersistence.Balance > 0)
            {
                throw new AccountBalanceNotZeroException(new List<Guid> { existingPersistence.Id });
            }

            existingPersistence.Status = AccountStatus.Closed;
            existingPersistence.ClosureDate = DateTime.Now;
            existingPersistence.ClosureReason = closureReason;

            _db.Accounts.Update(existingPersistence);
            await _db.SaveChangesAsync();

            return AccountMapper.ToDto(existingPersistence);
        }
    }
}
