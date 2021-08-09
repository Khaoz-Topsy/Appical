using System.Collections.Generic;
using Appical.Domain.Dto.Transaction;

namespace Appical.Domain.Dto.Account
{
    public class AccountOverviewDto: AccountDto
    {
        public List<TransactionDto> Transactions { get; set; }
    }
}
