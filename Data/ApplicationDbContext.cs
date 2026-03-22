using kanimeclothing.Models;
using Microsoft.EntityFrameworkCore;

namespace kanimeclothing.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the Product entity
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products", "dbo");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Category).HasMaxLength(100);
                entity.Property(e => e.Price).HasPrecision(18, 2);
                entity.Property(e => e.Url).HasMaxLength(500);
                entity.Property(e => e.Color).HasMaxLength(100);
                entity.Property(e => e.Size).HasMaxLength(50);
                entity.Property(e => e.Material).HasMaxLength(255);
                entity.Property(e => e.Measurements).HasMaxLength(255);
                entity.Property(e => e.Origin).HasMaxLength(100);
                entity.Property(e => e.Barcode).HasMaxLength(50);
                entity.Property(e => e.ProductCode).HasMaxLength(50);
            });
        }
    }
}
