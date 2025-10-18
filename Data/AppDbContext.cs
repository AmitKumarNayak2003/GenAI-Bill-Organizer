using Microsoft.EntityFrameworkCore;
using BillOrganizerAPI.Models;

namespace BillOrganizerAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Bill> Bills { get; set; }
        public DbSet<BillItem> BillItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Bill entity
            modelBuilder.Entity<Bill>()
                .HasKey(b => b.Id);

            modelBuilder.Entity<Bill>()
                .HasMany(b => b.Items)
                .WithOne()
                .HasForeignKey(bi => bi.BillId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure BillItem entity
            modelBuilder.Entity<BillItem>()
                .HasKey(bi => bi.Id);
        }
    }
}
