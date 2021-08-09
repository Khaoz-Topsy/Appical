using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Appical.Persistence.Entity
{
    public class AccountOwner
    {
        public const int NameMaxLength = 100;

        [Required]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; }

        #region Relationships

        public virtual ICollection<Account> Accounts { get; set; }

        public static void MapRelationships(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountOwner>().HasKey(p => p.Id);
        }

        #endregion
    }
}
