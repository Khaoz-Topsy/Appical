using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Appical.Domain.Dto.Account;
using Appical.Domain.Enum;

namespace Appical.Persistence.Repository.Interface
{
    public interface IAccountRepository : ICrudRepository<AccountDto>
    {
        Task<AccountDto> Create(Guid accountOwnerId);
        Task<List<AccountDto>> ReadForAccountOwner(Guid id);
        Task<AccountDto> CloseAccount(Guid id, ClosureReasonType closureReason);
    }
}
