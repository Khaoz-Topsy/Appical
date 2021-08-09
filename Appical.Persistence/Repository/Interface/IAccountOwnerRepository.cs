using System.Threading.Tasks;
using Appical.Domain.Dto.AccountOwner;

namespace Appical.Persistence.Repository.Interface
{
    public interface IAccountOwnerRepository: ICrudRepository<AccountOwnerDto>
    {
        Task<AccountOwnerDto> Create(CreateAccountOwnerDto dto);
    }
}
