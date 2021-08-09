using System;

namespace Appical.Domain.Dto.Account
{
    public class AccountTransactionDto
    {
        public Guid AccountOwnerId { get; set; }
        public Guid AccountId { get; set; }
        public Guid? PreviousTransactionId { get; set; }
        public decimal Amount { get; set; }
    }
}
