using System;
using System.Text.Json.Serialization;
using Appical.Domain.Enum;

namespace Appical.Domain.Dto.Account
{
    public class AccountDto
    {
        public Guid Id { get; set; }

        [JsonIgnore]
        public Guid AccountOwnerId { get; set; }

        public decimal Balance { get; set; }

        public AccountStatus Status { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ClosureReasonType? ClosureReason { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? ClosureDate { get; set; }
    }
}
