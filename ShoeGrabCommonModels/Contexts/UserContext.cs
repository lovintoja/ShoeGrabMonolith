using Microsoft.EntityFrameworkCore;

namespace ShoeGrabCommonModels.Contexts;

public class UserContext : DbContext
{
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<UserProfile> Profiles { get; set; }
    public virtual DbSet<Order> Orders { get; set; }
    public virtual DbSet<OrderItem> OrderItems { get; set; }
    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<Basket> Baskets { get; set; }
    public virtual DbSet<BasketItem> BasketItems { get; set; }

    public UserContext(DbContextOptions<UserContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Profile);
        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasMaxLength(50);
        modelBuilder.Entity<UserProfile>()
            .HasOne(u => u.User);

        modelBuilder.Entity<Order>(order =>
        {
            order.Property(o => o.Status)
                .HasConversion<string>()
                .HasMaxLength(50);

            order.OwnsOne(o => o.ShippingAddress);
            order.OwnsOne(o => o.PaymentInfo);
        });

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(oi => oi.OrderId);

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Product)
            .WithMany()
            .HasForeignKey(oi => oi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<BasketItem>()
            .HasKey(bi => bi.Id);

        modelBuilder.Entity<Basket>()
            .HasKey(b => b.Id);

        modelBuilder.Entity<Basket>()
            .HasOne(b => b.User)
            .WithMany()
            .HasForeignKey(bi => bi.UserId);

        modelBuilder.Entity<Basket>()
            .HasMany(b => b.Items)
            .WithOne(bi => bi.Basket)
            .HasForeignKey(bi => bi.BasketId);

        modelBuilder.Entity<BasketItem>()
            .HasOne(bi => bi.Product)
            .WithMany()
            .HasForeignKey(bi => bi.ProductId);

        modelBuilder.Entity<BasketItem>()
            .Property(bi => bi.Quantity)
            .IsRequired();
    }

    public override async ValueTask DisposeAsync()
    {
        Dispose();
        await Task.CompletedTask;
    }
}
