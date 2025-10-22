using ContractMonthlyClaimSystem.Models;
using Microsoft.EntityFrameworkCore;

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
                .WithOne(i => i.Claim)
                .HasForeignKey(i => i.ClaimId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Claim>()
                .HasMany(c => c.Documents)
                .WithOne(d => d.Claim)
                .HasForeignKey(d => d.ClaimId)
                .OnDelete(DeleteBehavior.Cascade);

            // Precision via fluent (alternative to attributes)
            modelBuilder.Entity<Claim>().Property(p => p.TotalAmount).HasPrecision(18, 2);
            modelBuilder.Entity<Claim>().Property(p => p.TotalHours).HasPrecision(9, 2);
            modelBuilder.Entity<ClaimItem>().Property(p => p.Hours).HasPrecision(9, 2);
            modelBuilder.Entity<ClaimItem>().Property(p => p.Rate).HasPrecision(18, 2);

            // If you want DB-side defaults (Oracle example):
            // modelBuilder.Entity<Claim>().Property(p => p.SubmittedDate)
            //     .HasDefaultValueSql("SYSTIMESTAMP");
            // modelBuilder.Entity<Document>().Property(p => p.UploadedDate)
            //     .HasDefaultValueSql("SYSTIMESTAMP");
        }

    }
}
