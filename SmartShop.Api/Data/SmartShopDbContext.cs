using Microsoft.EntityFrameworkCore;
using SmartShop.Api.Models;

namespace SmartShop.Api.Data
{
    public class SmartShopDbContext : DbContext
    {
        public SmartShopDbContext(DbContextOptions<SmartShopDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<Review> Reviews => Set<Review>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");
                entity.HasKey(e => e.ProductId);
                entity.Property(e => e.ProductId).ValueGeneratedNever();
                entity.Property(e => e.ProductName).HasMaxLength(128).IsRequired();
                entity.Property(e => e.Category).HasMaxLength(64).IsRequired();
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.Features).HasMaxLength(512).IsRequired();
                entity.Property(e => e.Description).HasColumnType("nvarchar(max)");
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.ToTable("Review");
                entity.HasKey(e => e.ReviewId);
                entity.Property(e => e.ReviewId).ValueGeneratedOnAdd();
                entity.Property(e => e.CustomerName).HasMaxLength(64).IsRequired();
                entity.Property(e => e.Content).HasMaxLength(1000).IsRequired();
                entity.Property(e => e.Rating).IsRequired();
                entity.Property(e => e.Sentiment).HasMaxLength(16).IsRequired();
                entity.Property(e => e.Score).IsRequired();
                entity.Property(e => e.Summary).HasMaxLength(256).IsRequired();
                entity.Property(e => e.CreatedAt).HasColumnType("datetime").HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.Product)
                      .WithMany(p => p.Reviews)
                      .HasForeignKey(e => e.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}

