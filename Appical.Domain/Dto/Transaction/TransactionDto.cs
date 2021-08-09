using System;

namespace Appical.Domain.Dto.Transaction
{
    public class TransactionDto
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public Guid? PreviousTransactionId { get; set; }
        public DateTime ActionDate { get; set; }
    }
}
