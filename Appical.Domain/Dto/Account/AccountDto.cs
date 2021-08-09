using System;
using Appical.Domain.Enum;
using Newtonsoft.Json;

namespace Appical.Domain.Dto.Account
{
    public class AccountDto
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
    }
}
