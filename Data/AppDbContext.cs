using BudgetTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Income> Incomes { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Investment> Investments { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Import> Imports { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure relationships and constraints if needed
            builder.Entity<Tag>()
                .Property(t => t.Context)
                .HasConversion<string>();


            // 18 number of digits which 2 are after decimal point
            builder.Entity<Expense>()
                .Property(e => e.Amount)
                .HasPrecision(18, 2);

            builder.Entity<Income>()
                .Property(i => i.Amount)
                .HasPrecision(18, 2);

            builder.Entity<Investment>()
                .Property(i => i.Amount)
                .HasPrecision(18, 2);

            builder.Entity<Investment>()
                .Property(i => i.CurrentValue)
                .HasPrecision(18, 2);
        }
    }
}
