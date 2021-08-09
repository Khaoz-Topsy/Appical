using System;
using System.Collections.Generic;
using Appical.Domain.Dto.Transaction;
using Appical.Domain.Enum;
using Newtonsoft.Json;

namespace Appical.Domain.Dto.Account
{
    public class AccountOverviewDto
    {
        public Guid Id { get; set; }

        [JsonIgnore]
        public Guid AccountOwnerId { get; set; }

        public decimal Balance { get; set; }

        public AccountStatus Status { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ClosureReasonType? ClosureReason { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? ClosureDate { get; set; }

        public List<TransactionDto> Transactions { get; set; }
    }
}
