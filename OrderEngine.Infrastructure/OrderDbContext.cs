using Microsoft.EntityFrameworkCore;
using OrderEngine.Domain;

namespace OrderEngine.Infrastructure;

public sealed class OrderDbContext : DbContext
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CustomerId).IsRequired();
            entity.Property(x => x.CreatedAt).IsRequired();
            entity.Property(x => x.Status).HasConversion<string>();

            entity.HasMany(x => x.Items)
                .WithOne()
                .HasForeignKey("OrderId")
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.ProductId).IsRequired();
            entity.Property(x => x.ProductName).IsRequired();
            entity.Property(x => x.Quantity).IsRequired();
            entity.Property(x => x.UnitPrice).HasPrecision(18, 2).IsRequired();
        });
    }
}
