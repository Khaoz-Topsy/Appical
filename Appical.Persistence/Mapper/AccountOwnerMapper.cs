using System;
using Appical.Domain.Dto.AccountOwner;
using Appical.Persistence.Entity;

namespace Appical.Persistence.Mapper
{
    public static class AccountOwnerMapper
    {
        public static AccountOwner ToPersistence(AccountOwnerDto dto)
        {
            return new AccountOwner
            {
                Id = dto.Id,
                Name = dto.Name,
            };
        }

        public static AccountOwnerDto Create(CreateAccountOwnerDto dto)
        {
            AccountOwnerDto dtoToCreate = new AccountOwnerDto
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
            };

            return dtoToCreate;
        }

        public static AccountOwner EditExisting(AccountOwner persistence, AccountOwnerDto dto)
        {
            persistence.Name = dto.Name;

            return persistence;
        }

        public static AccountOwnerDto ToDto(AccountOwner persistence)
        {
            return new AccountOwnerDto
            {
                Id = persistence.Id,
                Name = persistence.Name,
            };
        }
    }
}
