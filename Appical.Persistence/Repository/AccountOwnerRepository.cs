using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appical.Domain.Dto.AccountOwner;
using Appical.Domain.Exception;
using Appical.Persistence.Entity;
using Appical.Persistence.Mapper;
using Appical.Persistence.Repository.Interface;
using Appical.Persistence.Validator;
using Microsoft.EntityFrameworkCore;

namespace Appical.Persistence.Repository
{
    public class AccountOwnerRepository: IAccountOwnerRepository
    {
        private readonly AppicalContext _db;

        public AccountOwnerRepository(AppicalContext db)
        {
            _db = db;
        }


        /// <inheritdoc cref="Create(AccountOwnerDto)" />
        /// <summary>
        /// Create AccountOwner with default properties set
        /// </summary>
        /// <param name="dto">Reduced amount of properties required for creating an AccountOwner</param>
        public Task<AccountOwnerDto> Create(CreateAccountOwnerDto dto) => Create(AccountOwnerMapper.Create(dto));

        /// <summary>
        /// Create AccountOwner
        /// </summary>
        /// <param name="dto"></param>
        public async Task<AccountOwnerDto> Create(AccountOwnerDto dto)
        {
            dto.Validate();

            AccountOwner persistence = AccountOwnerMapper.ToPersistence(dto);
            await _db.AccountOwners.AddAsync(persistence);
            await _db.SaveChangesAsync();

            return AccountOwnerMapper.ToDto(persistence);
        }

        /// <summary>
        /// Read all existing AccountOwners
        /// </summary>
        public async Task<List<AccountOwnerDto>> Read()
        {
            List<AccountOwner> accountOwners = await _db.AccountOwners.ToListAsync();
            return accountOwners.Select(AccountOwnerMapper.ToDto).ToList();
        }

        /// <summary>
        /// Get specific AccountOwner
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="PersistenceEntityDoesNotExistException">AccountOwner doesn't exist for id specified.</exception>
        public async Task<AccountOwnerDto> Read(Guid id)
        {
            AccountOwner existingPersistence = await _db.AccountOwners.FirstOrDefaultAsync(ao => ao.Id.Equals(id));
            if (existingPersistence == null) throw new PersistenceEntityDoesNotExistException(id);
            
            return AccountOwnerMapper.ToDto(existingPersistence);
        }

        /// <summary>
        /// Update specific AccountOwner
        /// </summary>
        /// <param name="dto"></param>
        /// <exception cref="PersistenceEntityDoesNotExistException">AccountOwner doesn't exist for dto.Id.</exception>
        public async Task<AccountOwnerDto> Update(AccountOwnerDto dto)
        {
            AccountOwner existingPersistence = await _db.AccountOwners.FirstOrDefaultAsync(ao => ao.Id.Equals(dto.Id));
            if (existingPersistence == null) throw new PersistenceEntityDoesNotExistException(dto.Id);

            dto.Validate();

            AccountOwner alteredPersistence = AccountOwnerMapper.EditExisting(existingPersistence, dto);
            _db.AccountOwners.Update(alteredPersistence);
            await _db.SaveChangesAsync();

            return AccountOwnerMapper.ToDto(alteredPersistence);
        }

        /// <summary>
        /// Delete specific AccountOwner
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="PersistenceEntityDoesNotExistException">AccountOwner doesn't exist for id specified.</exception>
        /// <exception cref="AccountBalanceNotZeroException">Not all Accounts for AccountOwner have a balance of zero.</exception>
        public async Task<AccountOwnerDto> Delete(Guid id)
        {
            AccountOwner existingPersistence = await _db.AccountOwners.FirstOrDefaultAsync(ao => ao.Id.Equals(id));
            if (existingPersistence == null) throw new PersistenceEntityDoesNotExistException(id);

            List<Account> invalidAccounts = _db.Accounts.Where(acc => acc.Balance != 0).ToList();
            if (invalidAccounts.Count > 0)
            {
                throw new AccountBalanceNotZeroException(invalidAccounts.Select(ia => ia.Id).ToList());
            }

            _db.AccountOwners.Remove(existingPersistence);
            await _db.SaveChangesAsync();

            return AccountOwnerMapper.ToDto(existingPersistence);
        }
    }
}
