using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Appical.Domain.Enum;
using Microsoft.EntityFrameworkCore;

namespace Appical.Persistence.Entity
{
    public class Account
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid AccountOwnerId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal Balance { get; set; }

        [Required]
        public AccountStatus Status { get; set; }

        public ClosureReasonType? ClosureReason { get; set; }

        public DateTime? ClosureDate { get; set; }


        #region Relationships

        public virtual AccountOwner AccountOwner { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }

        public static void MapRelationships(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().HasKey(p => p.Id);

            modelBuilder.Entity<Account>()
                .HasOne(up => up.AccountOwner)
                .WithMany(b => b.Accounts)
                .HasForeignKey(up => up.AccountOwnerId)
                .HasConstraintName("ForeignKey_AccountOwner_Accounts");
        }

        #endregion
    }
}
