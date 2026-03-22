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
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

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

            // Configure the Order entity
            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Orders", "dbo");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CustomerEmail).HasMaxLength(255);
                entity.Property(e => e.CustomerPhone).HasMaxLength(20);
                entity.Property(e => e.PaystackReference).HasMaxLength(100).IsRequired();
                entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.CreatedDate).IsRequired();
                entity.HasMany(e => e.Items)
                    .WithOne(i => i.Order)
                    .HasForeignKey(i => i.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure the OrderItem entity
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.ToTable("OrderItems", "dbo");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ProductName).HasMaxLength(255);
                entity.Property(e => e.Price).HasPrecision(18, 2);
                entity.Property(e => e.Size).HasMaxLength(50);
                entity.Property(e => e.Color).HasMaxLength(100);
                entity.Property(e => e.ImageUrl).HasMaxLength(500);
                entity.HasOne(e => e.Order)
                    .WithMany(o => o.Items)
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
