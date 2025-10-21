using ContractMonthlyClaimSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace ContractMonthlyClaimSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Claim> Claims => Set<Claim>();
        public DbSet<ClaimItem> ClaimItems => Set<ClaimItem>();
        public DbSet<Document> Documents => Set<Document>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Claim>()
                .HasMany(c => c.ClaimItems)
                .WithOne()
                .HasForeignKey(i => i.ClaimId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Claim>()
                .HasMany(c => c.Documents)
                .WithOne()
                .HasForeignKey(d => d.ClaimId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
