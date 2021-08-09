using Appical.Persistence.Entity;
using Microsoft.EntityFrameworkCore;

namespace Appical.Persistence
{
    public class AppicalContext : DbContext
    {
        public DbSet<AccountOwner> AccountOwners { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }


        public AppicalContext(DbContextOptions<AppicalContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            AccountOwner.MapRelationships(modelBuilder);
            Account.MapRelationships(modelBuilder);
            Transaction.MapRelationships(modelBuilder);
        }
    }

}
