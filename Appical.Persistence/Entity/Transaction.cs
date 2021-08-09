using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Appical.Domain.Enum;
using Microsoft.EntityFrameworkCore;

namespace Appical.Persistence.Entity
{
    public class Transaction
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid AccountId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal Amount { get; set; }
        
        [Required]
        public TransactionActionType Action { get; set; }

        [Required]
        public DateTime ActionDate { get; set; }


        #region Relationships
        public virtual Account Account { get; set; }

        public static void MapRelationships(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transaction>().HasKey(p => p.Id);

            modelBuilder.Entity<Transaction>()
                .HasOne(up => up.Account)
                .WithMany(b => b.Transactions)
                .HasForeignKey(up => up.AccountId)
                .HasConstraintName("ForeignKey_Account_Transactions");
        }

        #endregion
    }
}
