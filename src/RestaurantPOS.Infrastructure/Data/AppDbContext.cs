using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RestaurantPOS.Core.Common;
using RestaurantPOS.Core.Entities;

namespace RestaurantPOS.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Table> Tables => Set<Table>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<MenuItemInventory> MenuItemInventories => Set<MenuItemInventory>();
    public DbSet<RestockLog> RestockLogs => Set<RestockLog>();
    public DbSet<Promotion> Promotions => Set<Promotion>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                builder.Entity(entityType.ClrType)
                    .Property(nameof(BaseEntity.CreatedAt))
                    .HasDefaultValueSql("datetime('now')");
            }
        }

        builder.Entity<MenuItem>(e =>
        {
            e.HasKey(m => m.Id);
            e.Property(m => m.Name).HasMaxLength(200).IsRequired();
            e.Property(m => m.Price).HasColumnType("decimal(18,2)");
        });

        builder.Entity<Order>(e =>
        {
            e.HasKey(o => o.Id);
            e.Property(o => o.SubTotal).HasColumnType("decimal(18,2)");
            e.Property(o => o.TaxAmount).HasColumnType("decimal(18,2)");
            e.Property(o => o.DiscountAmount).HasColumnType("decimal(18,2)");
            e.Property(o => o.TipAmount).HasColumnType("decimal(18,2)");
            e.HasOne(o => o.Table).WithMany(t => t.Orders).HasForeignKey(o => o.TableId);
            e.HasOne(o => o.Customer).WithMany(c => c.Orders).HasForeignKey(o => o.CustomerId).IsRequired(false);
            e.HasQueryFilter(o => !o.IsDeleted);
        });

        builder.Entity<OrderItem>(e =>
        {
            e.HasKey(oi => oi.Id);
            e.Property(oi => oi.UnitPrice).HasColumnType("decimal(18,2)");
            e.HasOne(oi => oi.Order).WithMany(o => o.Items).HasForeignKey(oi => oi.OrderId);
            e.HasOne(oi => oi.MenuItem).WithMany(m => m.OrderItems).HasForeignKey(oi => oi.MenuItemId);
            e.HasQueryFilter(oi => !oi.IsDeleted);
        });

        builder.Entity<Table>(e =>
        {
            e.HasKey(t => t.Id);
            e.Property(t => t.TableNumber).HasMaxLength(20).IsRequired();
            e.HasQueryFilter(t => !t.IsDeleted);
        });

        builder.Entity<Payment>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Amount).HasColumnType("decimal(18,2)");
            e.Property(p => p.TipAmount).HasColumnType("decimal(18,2)");
            e.HasOne(p => p.Order).WithOne(o => o.Payment).HasForeignKey<Payment>(p => p.OrderId);
            e.HasQueryFilter(p => !p.IsDeleted);
        });

        builder.Entity<Customer>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.FullName).HasMaxLength(200).IsRequired();
            e.Property(c => c.Email).HasMaxLength(200);
            e.Property(c => c.Phone).HasMaxLength(20);
            e.HasQueryFilter(c => !c.IsDeleted);
        });

        builder.Entity<InventoryItem>(e =>
        {
            e.HasKey(i => i.Id);
            e.Property(i => i.Name).HasMaxLength(200).IsRequired();
            e.Property(i => i.CostPerUnit).HasColumnType("decimal(18,2)");
        });

        builder.Entity<MenuItemInventory>(e =>
        {
            e.HasKey(mi => new { mi.MenuItemId, mi.InventoryItemId });
            e.HasOne(mi => mi.MenuItem).WithMany(m => m.MenuItemInventories).HasForeignKey(mi => mi.MenuItemId);
            e.HasOne(mi => mi.InventoryItem).WithMany(i => i.MenuItemInventories).HasForeignKey(mi => mi.InventoryItemId);
            e.Property(mi => mi.QuantityRequired).HasColumnType("decimal(18,4)");
        });

        builder.Entity<RestockLog>(e =>
        {
            e.HasKey(r => r.Id);
            e.Property(r => r.CostPerUnit).HasColumnType("decimal(18,2)");
            e.HasOne(r => r.InventoryItem).WithMany(i => i.RestockLogs).HasForeignKey(r => r.InventoryItemId);
            e.HasQueryFilter(r => !r.IsDeleted);
        });

        builder.Entity<Promotion>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Code).HasMaxLength(50).IsRequired();
            e.Property(p => p.DiscountPercent).HasColumnType("decimal(5,2)");
            e.Property(p => p.DiscountAmount).HasColumnType("decimal(18,2)");
            e.HasQueryFilter(p => !p.IsDeleted);
        });

        SeedData.Seed(builder);
    }
}
